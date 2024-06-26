using Tracker.Data;

namespace Tracker.RavenDb;

internal class Peer
{
    public string Hash { get; set; }
    public string IP { get; set; }
    public int Port { get; set; }
    public PeerType PeerType { get; set; }
}