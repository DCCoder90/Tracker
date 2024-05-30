using Tracker.Net.Web.Data;

namespace Tracker.Net.Web;

public interface IWebRepository
{
    /// <summary>
    /// Returns a collection of all currently tracked torrents
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IEnumerable<Torrent>> GetAllTorrents(CancellationToken cancellationToken = new());

    /// <summary>
    /// Retrieves a single tracked torrent
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Torrent> GetTorrent(string hash, CancellationToken cancellationToken = new());

    /// <summary>
    /// Add torrent file details to backing
    /// </summary>
    /// <param name="torrent"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task AddTorrent(Torrent torrent, CancellationToken cancellationToken = new());

    /// <summary>
    /// Remove a torrent from backing
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task RemoveTorrent(string hash, CancellationToken cancellationToken = new());
    
    /// <summary>
    /// Returns a collection of all peers for a supplied torrent hash
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IEnumerable<TorrentPeer>> GetTorrentPeers(string hash, CancellationToken cancellationToken = new());
}