using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Tracker.Data;
using Tracker.Data.Repository;
using Tracker.Data.Torrent;
using Tracker.Data.Util;

namespace Tracker.RavenDb;

public class RavenRepository : IRepository
{
    public string BackingType => "RavenDB";
    private IDocumentStore _store;

    public RavenRepository()
    {
        _store = DocumentStoreHolder.Store;
    }
    public void ResetHash(string hash)
    {
        using IDocumentSession session = _store.OpenSession();
        var item = session.Load<Torrent>(hash);
        item.Seeders = 0;
        item.Leechers = 0;
        session.SaveChanges();
    }

    public void AddPeer(TorrentPeer peer, byte[] hash, PeerType type = PeerType.Seeder)
    {
        using IDocumentSession session = _store.OpenSession();
        var storedPeer = new Peer
        {
            Hash = Unpack.Hex(hash),
            IP = peer.GetIPString(),
            Port = peer.Port,
            PeerType = type
        };
        session.Store(peer);
        session.SaveChanges();
    }

    public void RemovePeer(TorrentPeer peer, byte[] hash, PeerType type = PeerType.Seeder)
    {
        using IDocumentSession session = _store.OpenSession();
        var hashString = Unpack.Hex(hash);

        var peerToDelete = session.Query<Peer>()
            .SingleOrDefault(x => x.Hash == hashString && x.IP == peer.GetIPString());
        
        session.Delete(peerToDelete);
        session.SaveChanges();
    }

    public List<TorrentPeer> GetPeers(byte[] hash)
    {
        using IDocumentSession session = _store.OpenSession();
        var hashString = Unpack.Hex(hash);
        var peers = session.Query<Peer>().Where(x => x.Hash == hashString).ToList();

        return peers.Select(peer => new TorrentPeer(peer.IP, peer.Port)).ToList();
    }

    public List<TorrentInfo> ScrapeHashes(List<byte[]> hashes)
    {
        using IDocumentSession session = _store.OpenSession();

        var torrentList = new List<TorrentInfo>();
        foreach (var hash in hashes)
        {
            var hashString = Unpack.Hex(hash);
            var peers = session.Query<Peer>().Where(x => x.Hash == hashString).ToList();

            torrentList.Add(new TorrentInfo(hash,(uint)peers.Count(x=>x.PeerType == PeerType.Seeder),
                (uint)peers.Count(x=>x.PeerType == PeerType.Seeder),
                (uint)peers.Count(x=>x.PeerType == PeerType.Leecher)));
        }

        return torrentList;
    }
}