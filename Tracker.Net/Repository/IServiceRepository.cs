using Tracker.Data.Torrent;

namespace Tracker.Data.Repository;

public interface IServiceRepository
{
    /// <summary>
    /// The name of the backing type
    /// </summary>
    string BackingType { get; }
    
    /// <summary>
    /// Set seeder and leecher count to 0 for torrent
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="cancellationToken"></param>
    Task ResetHash(string hash, CancellationToken cancellationToken = new ());
    
    /// <summary>
    /// Add new peer for torrent
    /// </summary>
    /// <param name="peer"></param>
    /// <param name="connectionId"></param>
    /// <param name="hash"></param>
    /// <param name="type"></param>
    /// <param name="cancellationToken"></param>
    Task AddPeer(TorrentPeer peer, ulong connectionId, byte[] hash, PeerType type = PeerType.Seeder, CancellationToken cancellationToken = new ());
    
    /// <summary>
    /// Remove peer from torrent
    /// </summary>
    /// <param name="peer"></param>
    /// <param name="connectionId"></param>
    /// <param name="hash"></param>
    /// <param name="type"></param>
    /// <param name="cancellationToken"></param>
    Task RemovePeer(TorrentPeer peer, ulong connectionId, byte[] hash, PeerType type = PeerType.Seeder, CancellationToken cancellationToken = new ());
    
    /// <summary>
    /// Retrieve a list of peers for given hash
    /// </summary>
    /// <param name="hash">Torrent hash</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<TorrentPeer>> GetPeers(byte[] hash, CancellationToken cancellationToken = new ());
    
    /// <summary>
    /// Retrieve details of torrent for list of hashes
    /// </summary>
    /// <param name="hashes">List of torrent hashes</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<TorrentInfo>> ScrapeHashes(List<byte[]> hashes, CancellationToken cancellationToken = new ());
    
    /// <summary>
    /// Remove stale peers from backing
    /// </summary>
    /// <param name="tilStale"></param>
    /// <param name="cancellationToken"></param>
    Task ClearStale(TimeSpan tilStale, CancellationToken cancellationToken = new ());
}