using Tracker.Filesys.Torrent;
using Tracker.Net.Torrent;

namespace Tracker.Net.Repository;

public interface IWebRepository
{
    /// <summary>
    /// Returns a collection of all currently tracked torrents
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IEnumerable<TorrentInfo>> GetAllTorrents(CancellationToken cancellationToken = new());

    /// <summary>
    /// Add torrent file details to backing
    /// </summary>
    /// <param name="torrent"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task AddTorrent(TorrentFile torrent, CancellationToken cancellationToken = new());
    
    /// <summary>
    /// Returns a collection of all peers for a supplied torrent hash
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IEnumerable<TorrentPeer>> GetTorrentPeers(string hash, CancellationToken cancellationToken = new());
}