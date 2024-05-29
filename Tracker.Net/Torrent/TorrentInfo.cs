using Tracker.Data.Util;

namespace Tracker.Data.Torrent;

public class TorrentInfo
{
    public uint Completed;
    public byte[] InfoHash;
    public uint Leechers;

    public List<TorrentPeer> Peers;
    public uint Seeders;

    public TorrentInfo(byte[] hash, uint seeders, uint completed, uint leechers)
    {
        InfoHash = hash;
        Seeders = seeders;
        Completed = completed;
        Leechers = leechers;
    }

    public byte[] PackedTorrentInfo()
    {
        return Pack.UInt32(Seeders).Concat(Pack.UInt32(Completed)).Concat(Pack.UInt32(Leechers)).ToArray();
    }
}