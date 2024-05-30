using StackExchange.Redis;
using Tracker.Net;
using Tracker.Net.Repository;
using Tracker.Net.Torrent;
using Tracker.Net.Util;

namespace Tracker.Redis;

public class RedisServiceRepository : IServiceRepository
{
    private readonly IConnectionMultiplexer _backing;

    public RedisServiceRepository(BackingOptions backingOptions)
    {
        var options = new ConfigurationOptions
        {
            EndPoints = new EndPointCollection
            {
                $"{backingOptions.Host}:{backingOptions.Port}"
            }
        };

        if (backingOptions.UsesAuthentication)
        {
            options.Password = backingOptions.Password;
            options.User = backingOptions.User;
        }
        _backing = ConnectionMultiplexer.Connect(options);
    }

    public string BackingType => "Redis";

    public async Task ResetHash(string hash, CancellationToken cancellationToken = new ())
    {
        var db = _backing.GetDatabase();
        var nullCheck = await db.StringGetAsync("s:" + hash);
        if (nullCheck.IsNullOrEmpty)
        {
            await db.StringSetAsync("s:" + hash, "0");
            await db.StringSetAsync("l:" + hash, "0");
        }
    }

    //TODO: Implement connectionId
    public async Task AddPeer(TorrentPeer peer, ulong connectionId, byte[] hash, PeerType type = PeerType.Seeder, CancellationToken cancellationToken = new ())
    {
        var db = _backing.GetDatabase();
        var insert = peer.StringPeer();
        var stringHash = Unpack.Hex(hash);

        await db.SetAddAsync($"t:{stringHash}", insert);

        if (type == PeerType.Seeder)
            await db.StringIncrementAsync($"s:{stringHash}"); //amount of seeders
        else
            await db.StringIncrementAsync($"l:{stringHash}"); //amount of leechers
    }

    //TODO: Implement connectionId
    public async Task RemovePeer(TorrentPeer peer, ulong connectionId, byte[] hash, PeerType type = PeerType.Seeder, CancellationToken cancellationToken = new ())
    {
        var db = _backing.GetDatabase();
        var insert = peer.StringPeer();
        var stringHash = Unpack.Hex(hash);

        db.SetRemove("t:" + Unpack.Hex(hash), insert);

        if (type == PeerType.Seeder)
            await db.StringDecrementAsync($"s:{stringHash}");
        else
            await db.StringDecrementAsync($"l:{stringHash}");
    }

    public async Task<List<TorrentPeer>> GetPeers(byte[] hash, CancellationToken cancellationToken = new ())
    {
        var peers = new List<TorrentPeer>();
        var db = _backing.GetDatabase();
        var value = await db.SetMembersAsync($"t:{Unpack.Hex(hash)}");

        foreach (var peer in value)
            if (peer.HasValue)
            {
                var peerResponse = (string)peer;
                peers.Add(new TorrentPeer(peerResponse));
            }

        return peers;
    }

    public async Task<List<TorrentInfo>> ScrapeHashes(List<byte[]> hashes, CancellationToken cancellationToken = new ())
    {
        var list = new List<TorrentInfo>();
        var db = _backing.GetDatabase();
        foreach (var hash in hashes)
        {
            var hashString = BitConverter.ToString(hash).Replace("-", string.Empty);
            var seeders = await db.StringGetAsync($"s:{hashString}");
            var leechers = await db.StringGetAsync($"l:{hashString}");

            if (seeders.IsNullOrEmpty)
                seeders = 0;

            if (leechers.IsNullOrEmpty)
                leechers = 0;

            list.Add(new TorrentInfo(hash, uint.Parse(seeders), uint.Parse(seeders), uint.Parse(leechers)));
        }

        return list;
    }

    public async Task ClearStale(TimeSpan tilStale, CancellationToken cancellationToken = new ())
    {
        throw new NotImplementedException();
    }
}