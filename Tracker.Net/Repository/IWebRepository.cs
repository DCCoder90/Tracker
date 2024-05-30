using Tracker.Data.Torrent;

namespace Tracker.Data.Repository;

public interface IWebRepository
{
    /// <summary>
    /// Returns a collection of all currently tracked torrents
    /// </summary>
    /// <returns></returns>
    public IEnumerable<TorrentInfo> GetAllTorrents();
    
    /// <summary>
    /// Returns a collection of all peers for a supplied torrent hash
    /// </summary>
    /// <param name="hash"></param>
    /// <returns></returns>
    public IEnumerable<TorrentPeer> GetTorrentPeers(string hash);
}