using System.Net;
using System.Net.Sockets;
using System.Text;
using Tracker.Net;
using Tracker.Net.Packets;
using Tracker.Net.Repository;
using Tracker.Net.Torrent;
using Tracker.Net.Util;
using Action = Tracker.Net.Action;

namespace Tracker.Service;

public class Worker : BackgroundService
{
    private readonly int _announceInterval;
    private readonly int _maxAttempts;
    private readonly ILogger<Worker> _logger;
    private readonly IServiceRepository _serviceRepository;
    private readonly UdpClient _udpClient;
    private ServiceState _state;

    public Worker(IServiceRepository serviceRepository, ILogger<Worker> logger, WorkerOptions options)
    {
        _announceInterval = options.AnnounceInterval;
        _maxAttempts = options.MaxAttempts;
        
        var localEndpoint = new IPEndPoint(IPAddress.Any, options.Port);
        _udpClient = new UdpClient(localEndpoint);
        _serviceRepository = serviceRepository;

        _logger = logger;

        logger.LogInformation("Starting tracker");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (_state != ServiceState.Stopped)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                _state = ServiceState.Stopped;
                continue;
            }

            if (_state == ServiceState.Paused)
            {
                await Task.Delay(1000, stoppingToken);
                continue;
            }

            var receivedResults = await _udpClient.ReceiveAsync(stoppingToken);
            if (receivedResults.Buffer.Length > 0)
                await ReceivedData(receivedResults, stoppingToken);
            
            _serviceRepository.ClearStale(TimeSpan.FromSeconds(_announceInterval*_maxAttempts));
        }
    }

    private async Task ReceivedData(UdpReceiveResult res, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received information");
        var receivedData = res.Buffer;
        var endPointAddress = res.RemoteEndPoint.Address;
        var addressString = endPointAddress.ToString();

        if (receivedData.Length > 12)
        {
            var action = (Action)Unpack.UInt32(receivedData, 8);
            switch (action)
            {
                case Action.Connect:
                    var connectRequest = new ConnectRequest(receivedData);
                    _logger.LogInformation($"[Connect] from {addressString} :{res.RemoteEndPoint.Port}");

                    var connectResponse = new ConnectResponse(0, connectRequest.TransactionID, 13376969);
                    await SendDataAsync(_udpClient, connectResponse.Data, res.RemoteEndPoint);
                    break;


                case Action.Announce:
                    var announceRequest = new AnnounceRequest(receivedData);
                    _logger.LogInformation(
                        $"[Announce] from {addressString}:{announceRequest.Port} {(Event)announceRequest.TorrentEvent}");

                    var peer = new TorrentPeer(addressString, announceRequest.Port);

                    if ((Event)announceRequest.TorrentEvent == Event.Stopped)
                    {
                        await _serviceRepository.RemovePeer(peer, announceRequest.ConnectionID, announceRequest.InfoHash);
                    }
                    else
                    {
                        var type = announceRequest.Left > 0 ? PeerType.Leecher : PeerType.Seeder;
                        if ((Event)announceRequest.TorrentEvent == Event.Unknown)
                            type = PeerType.Seeder;
                        _serviceRepository.AddPeer(peer, announceRequest.ConnectionID, announceRequest.InfoHash,type);
                    }

                    var peers = await _serviceRepository.GetPeers(announceRequest.InfoHash);
                    var torrentInfo = await _serviceRepository.ScrapeHashes(new List<byte[]> { announceRequest.InfoHash });
                    var seeders = torrentInfo.First().Seeders;
                    var leechers = torrentInfo.First().Leechers;
                    var announceResponse = new AnnounceResponse(announceRequest.TransactionID, (uint)_announceInterval,
                        leechers, seeders, peers);
                    await SendDataAsync(_udpClient, announceResponse.Data, res.RemoteEndPoint);
                    break;


                case Action.Scrape:
                    var scrapeRequest = new ScrapeRequest(receivedData);
                    _logger.LogInformation(
                        $"[Scrape] from {addressString} for {scrapeRequest.InfoHashes.Count} torrents");

                    var scrapedTorrents = await _serviceRepository.ScrapeHashes(scrapeRequest.InfoHashes);
                    var scrapeResponse = new ScrapeResponse(scrapeRequest.TransactionID, scrapedTorrents);

                    await SendDataAsync(_udpClient, scrapeResponse.Data, res.RemoteEndPoint);

                    break;
                default:
                    _logger.LogWarning($"Unknown data: {Encoding.UTF8.GetString(receivedData)}");
                    break;
            }
        }
        else
        {
            _logger.LogWarning($"Unknown data: {Encoding.UTF8.GetString(receivedData)}");
        }
    }

    private static async Task SendDataAsync(UdpClient client, byte[] data, IPEndPoint endpoint)
    {
        await client.SendAsync(data, data.Length, endpoint);
    }
}