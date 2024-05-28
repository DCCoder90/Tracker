using StackExchange.Redis;
using Tracker.Data;
using Tracker.Data.Repository;
using Tracker.Data.Torrent;
using Tracker.Data.Util;

namespace Tracker.Redis;

public class RedisRepository : IRepository
{
    public string BackingType => "Redis";

    private readonly ConnectionMultiplexer _backing;

    public RedisRepository()
    {
        var options = new ConfigurationOptions
        {
            Password = "redispw",
            User = "default",
            EndPoints = new EndPointCollection
            {
                "localhost:34147"
            }
        };
        _backing = ConnectionMultiplexer.Connect(options);
    }

    public void ResetHash(string hash)
    {
        var db = _backing.GetDatabase();
        var nullCheck = db.StringGet("s:" + hash);
        if (nullCheck.IsNullOrEmpty)
        {
            db.StringSet("s:" + hash, "0");
            db.StringSet("l:" + hash, "0");
        }
    }

    public void AddPeer(TorrentPeer peer, byte[] hash, PeerType type = PeerType.Seeder)
    {
        var db = _backing.GetDatabase();
        var insert = peer.StringPeer();
        var stringHash = Unpack.Hex(hash);

        db.SetAdd("t:" + stringHash, insert);


        if (type == PeerType.Seeder)
            db.StringIncrement("s:" + stringHash); //amount of seeders
        else
            db.StringIncrement("l:" + stringHash); //amount of leechers
    }


    public void RemovePeer(TorrentPeer peer, byte[] hash, PeerType type = PeerType.Seeder)
    {
        var db = _backing.GetDatabase();
        var insert = peer.StringPeer();
        var stringHash = Unpack.Hex(hash);

        db.SetRemove("t:" + Unpack.Hex(hash), insert);

        if (type == PeerType.Seeder)
            db.StringDecrement("s:" + Unpack.Hex(hash));
        else
            db.StringDecrement("l:" + Unpack.Hex(hash));
    }

    public List<TorrentPeer> GetPeers(byte[] hash)
    {
        var peers = new List<TorrentPeer>();
        var db = _backing.GetDatabase();
        var value = db.SetMembers("t:" + Unpack.Hex(hash));

        foreach (var peer in value)
            if (peer.HasValue)
            {
                var peerResponse = (string)peer;
                peers.Add(new TorrentPeer(peerResponse));
            }

        return peers;
    }

    public List<TorrentInfo> ScrapeHashes(List<byte[]> hashes)
    {
        var list = new List<TorrentInfo>();
        var db = _backing.GetDatabase();
        foreach (var hash in hashes)
        {
            var hashString = BitConverter.ToString(hash).Replace("-", string.Empty);
            var seeders = db.StringGet($"s:{hashString}");
            var leechers = db.StringGet($"l:{hashString}");

            if (seeders.IsNullOrEmpty)
                seeders = 0;

            if (leechers.IsNullOrEmpty)
                leechers = 0;

            list.Add(new TorrentInfo(hash, uint.Parse(seeders), uint.Parse(seeders), uint.Parse(leechers)));
        }

        return list;
    }
}