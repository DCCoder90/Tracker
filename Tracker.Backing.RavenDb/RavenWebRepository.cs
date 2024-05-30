using Tracker.Net.Repository;
using Tracker.Net.Web;
using TorrentPeer = Tracker.Net.Web.Data.TorrentPeer;
using Torrent = Tracker.Net.Web.Data.Torrent;

namespace Tracker.RavenDb;

public class RavenWebRepository : IWebRepository
{
    public Task<IEnumerable<Torrent>> GetAllTorrents(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task AddTorrent(Torrent torrent, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TorrentPeer>> GetTorrentPeers(string hash, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
}