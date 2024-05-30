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
    void ResetHash(string hash);
    
    /// <summary>
    /// Add new peer for torrent
    /// </summary>
    /// <param name="peer"></param>
    /// <param name="connectionId"></param>
    /// <param name="hash"></param>
    /// <param name="type"></param>
    void AddPeer(TorrentPeer peer, ulong connectionId, byte[] hash, PeerType type = PeerType.Seeder);
    
    /// <summary>
    /// Remove peer from torrent
    /// </summary>
    /// <param name="peer"></param>
    /// <param name="connectionId"></param>
    /// <param name="hash"></param>
    /// <param name="type"></param>
    void RemovePeer(TorrentPeer peer, ulong connectionId, byte[] hash, PeerType type = PeerType.Seeder);
    
    /// <summary>
    /// Retrieve a list of peers for given hash
    /// </summary>
    /// <param name="hash">Torrent hash</param>
    /// <returns></returns>
    List<TorrentPeer> GetPeers(byte[] hash);
    
    /// <summary>
    /// Retrieve details of torrent for list of hashes
    /// </summary>
    /// <param name="hashes">List of torrent hashes</param>
    /// <returns></returns>
    List<TorrentInfo> ScrapeHashes(List<byte[]> hashes);
    
    /// <summary>
    /// Remove stale peers from backing
    /// </summary>
    /// <param name="tilStale"></param>
    void ClearStale(TimeSpan tilStale);
}