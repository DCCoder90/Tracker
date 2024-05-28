using Tracker.Data.Torrent;

namespace Tracker.Data.Repository;

public interface IRepository
{
    string BackingType { get; }
    void ResetHash(string hash);
    void AddPeer(TorrentPeer peer, byte[] hash, PeerType type = PeerType.Seeder);
    void RemovePeer(TorrentPeer peer, byte[] hash, PeerType type = PeerType.Seeder);
    List<TorrentPeer> GetPeers(byte[] hash);
    List<TorrentInfo> ScrapeHashes(List<byte[]> hashes);
}