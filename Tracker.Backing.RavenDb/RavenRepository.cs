using Raven.Client.Documents;
using Tracker.Data;
using Tracker.Data.Repository;
using Tracker.Data.Torrent;
using Tracker.Data.Util;

namespace Tracker.RavenDb;

public class RavenRepository : IRepository
{
    private readonly IDocumentStore _store;

    public RavenRepository()
    {
        _store = DocumentStoreHolder.Store;
    }

    public string BackingType => "RavenDB";

    public void ResetHash(string hash)
    {
        using var session = _store.OpenSession();
        var item = session.Load<Torrent>(hash);
        item.Seeders = 0;
        item.Leechers = 0;
        session.SaveChanges();
    }

    public void AddPeer(TorrentPeer peer, ulong connectionId, byte[] hash, PeerType type = PeerType.Seeder)
    {
        using var session = _store.OpenSession();
        var storedPeer = new Peer
        {
            Hash = Unpack.Hex(hash),
            IP = peer.GetIPString(),
            Port = peer.Port,
            PeerType = type,
            ConnectionId = connectionId,
            Created = DateTimeOffset.Now
        };
        session.Store(storedPeer);
        session.SaveChanges();
    }

    public void RemovePeer(TorrentPeer peer, ulong connectionId, byte[] hash, PeerType type = PeerType.Seeder)
    {
        using var session = _store.OpenSession();
        var hashString = Unpack.Hex(hash);

        var peerToDelete = session.Query<Peer>()
            .SingleOrDefault(x => x.Hash == hashString && x.IP == peer.GetIPString() && x.ConnectionId == connectionId);

        if (peerToDelete == null)
            return;
        
        session.Delete(peerToDelete);
        session.SaveChanges();
    }

    public List<TorrentPeer> GetPeers(byte[] hash)
    {
        using var session = _store.OpenSession();
        var hashString = Unpack.Hex(hash);
        var peers = session.Query<Peer>().Where(x => x.Hash == hashString).ToList();

        return peers.Select(peer => new TorrentPeer(peer.IP, peer.Port)).ToList();
    }

    public List<TorrentInfo> ScrapeHashes(List<byte[]> hashes)
    {
        using var session = _store.OpenSession();

        var torrentList = new List<TorrentInfo>();
        foreach (var hash in hashes)
        {
            var hashString = Unpack.Hex(hash);
            var peers = session.Query<Peer>().Where(x => x.Hash == hashString).ToList();

            torrentList.Add(new TorrentInfo(hash, (uint)peers.Count(x => x.PeerType == PeerType.Seeder),
                (uint)peers.Count(x => x.PeerType == PeerType.Seeder),
                (uint)peers.Count(x => x.PeerType == PeerType.Leecher)));
        }

        return torrentList;
    }

    public void ClearStale(TimeSpan tilStale)
    {
        using IDocumentSession session = _store.OpenSession();
        
        var peers = session.Query<Peer>().ToList();

        foreach (var peer in peers)
        {
            var diff = DateTimeOffset.Now - peer.Created;
            if(diff > tilStale)
                session.Delete(peer);
        }
        
        session.SaveChanges();
    }
}