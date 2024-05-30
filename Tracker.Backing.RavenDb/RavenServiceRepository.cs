using Raven.Client.Documents;
using Tracker.Net;
using Tracker.Net.Repository;
using Tracker.Net.Torrent;
using Tracker.Net.Util;
using Tracker.RavenDb.Data;
using TorrentPeer = Tracker.Net.Torrent.TorrentPeer;

namespace Tracker.RavenDb;

public class RavenServiceRepository : IServiceRepository
{
    private readonly IDocumentStore _store;

    public RavenServiceRepository(BackingOptions backingOptions)
    {
        _store = DocumentStoreHolder.GetStore(backingOptions);
    }

    public string BackingType => "RavenDB";

    public async Task ResetHash(string hash, CancellationToken cancellationToken = new ())
    {
        using var session = _store.OpenAsyncSession();
        var item = await session.LoadAsync<TorrentData>(hash, cancellationToken);
        item.Seeders = 0;
        item.Leechers = 0;
        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task AddPeer(TorrentPeer peer, ulong connectionId, byte[] hash, PeerType type = PeerType.Seeder, CancellationToken cancellationToken = new ())
    {
        if (await UpdatePeer(Unpack.Hex(hash), connectionId, cancellationToken))
            return;
        
        using var session = _store.OpenAsyncSession();
        var storedPeer = new Peer
        {
            Hash = Unpack.Hex(hash),
            IP = peer.GetIPString(),
            Port = peer.Port,
            PeerType = type,
            ConnectionId = connectionId,
            Created = DateTimeOffset.Now
        };
        await session.StoreAsync(storedPeer,cancellationToken);
        await session.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Update peer timestamp if it exists
    /// </summary>
    /// <param name="hashString"></param>
    /// <param name="connectionId"></param>
    /// <returns></returns>
    private async Task<bool> UpdatePeer(string hashString, ulong connectionId, CancellationToken cancellationToken = new ())
    {
        using var session = _store.OpenAsyncSession();
        var peerToUpdate = session.Query<Peer>()
            .SingleOrDefault(x => x.Hash == hashString && x.ConnectionId == connectionId);
        if (peerToUpdate != null)
        {
            peerToUpdate.Created = DateTimeOffset.Now;
            await session.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }
    
    public async Task RemovePeer(TorrentPeer peer, ulong connectionId, byte[] hash, PeerType type = PeerType.Seeder, CancellationToken cancellationToken = new ())
    {
        using var session = _store.OpenAsyncSession();
        var hashString = Unpack.Hex(hash);

        var peerToDelete = session.Query<Peer>()
            .SingleOrDefault(x => x.Hash == hashString && x.IP == peer.GetIPString() && x.ConnectionId == connectionId);

        if (peerToDelete == null)
            return;
        
        session.Delete(peerToDelete);
        await session.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<TorrentPeer>> GetPeers(byte[] hash, CancellationToken cancellationToken = new ())
    {
        using var session = _store.OpenAsyncSession();
        var hashString = Unpack.Hex(hash);
        var peers = await session.Query<Peer>().Where(x => x.Hash == hashString).ToListAsync();

        return peers.Select(peer => new TorrentPeer(peer.IP, peer.Port)).ToList();
    }

    public async Task<List<TorrentInfo>> ScrapeHashes(List<byte[]> hashes, CancellationToken cancellationToken = new ())
    {
        using var session = _store.OpenAsyncSession();

        var torrentList = new List<TorrentInfo>();
        foreach (var hash in hashes)
        {
            var hashString = Unpack.Hex(hash);
            var peers = await session.Query<Peer>().Where(x => x.Hash == hashString).ToListAsync(cancellationToken);

            torrentList.Add(new TorrentInfo(hash, (uint)peers.Count(x => x.PeerType == PeerType.Seeder),
                (uint)peers.Count(x => x.PeerType == PeerType.Seeder),
                (uint)peers.Count(x => x.PeerType == PeerType.Leecher)));
        }

        return torrentList;
    }

    public async Task ClearStale(TimeSpan tilStale, CancellationToken cancellationToken = new ())
    {
        using var session = _store.OpenAsyncSession();
        
        var peers = await session.Query<Peer>().ToListAsync(cancellationToken);

        foreach (var peer in from peer in peers let diff = DateTimeOffset.Now - peer.Created where diff > tilStale select peer)
        {
            session.Delete(peer);
        }
        
        await session.SaveChangesAsync(cancellationToken);
    }
}