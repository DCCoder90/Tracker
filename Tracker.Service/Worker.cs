using System.Net;
using System.Net.Sockets;
using System.Text;
using Tracker.Data;
using Tracker.Data.Packets;
using Tracker.Data.Repository;
using Tracker.Data.Torrent;
using Tracker.Data.Util;
using Action = Tracker.Data.Action;

namespace Tracker.Service;

public class Worker : BackgroundService
{
    private readonly uint _announceInterval = 60;
    private readonly int _maxAttempts = 8; //Set by spec
    private readonly ILogger<Worker> _logger;
    private readonly IRepository _repository;
    private ServiceState _state;
    private readonly UdpClient _udpClient;

    public Worker(IRepository repository, ILogger<Worker> logger)
    {
        var port = 55551;
        var localEndpoint = new IPEndPoint(IPAddress.Any, port);
        _udpClient = new UdpClient(localEndpoint);
        _repository = repository;

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
                await ReceivedData(receivedResults);
            
            _repository.ClearStale(TimeSpan.FromSeconds(_announceInterval*_maxAttempts));
        }
    }

    private async Task ReceivedData(UdpReceiveResult res)
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
                        _repository.RemovePeer(peer, announceRequest.ConnectionID, announceRequest.InfoHash);
                    }
                    else
                    {
                        var type = announceRequest.Left > 0 ? PeerType.Leecher : PeerType.Seeder;
                        if ((Event)announceRequest.TorrentEvent == Event.Unknown)
                            type = PeerType.Seeder;
                        _repository.AddPeer(peer, announceRequest.ConnectionID, announceRequest.InfoHash,type);
                    }

                    var peers = _repository.GetPeers(announceRequest.InfoHash);
                    var torrentInfo = _repository.ScrapeHashes(new List<byte[]> { announceRequest.InfoHash });
                    var seeders = torrentInfo.First().Seeders;
                    var leechers = torrentInfo.First().Leechers;
                    var announceResponse = new AnnounceResponse(announceRequest.TransactionID, _announceInterval,
                        leechers, seeders, peers);
                    await SendDataAsync(_udpClient, announceResponse.Data, res.RemoteEndPoint);
                    break;


                case Action.Scrape:
                    var scrapeRequest = new ScrapeRequest(receivedData);
                    _logger.LogInformation(
                        $"[Scrape] from {addressString} for {scrapeRequest.InfoHashes.Count} torrents");

                    var scrapedTorrents = _repository.ScrapeHashes(scrapeRequest.InfoHashes);
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