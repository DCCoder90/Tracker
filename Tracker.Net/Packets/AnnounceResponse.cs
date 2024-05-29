using Tracker.Data.Torrent;
using Tracker.Data.Util;

namespace Tracker.Data.Packets;

public class AnnounceResponse : Packet
{
    public byte[] Data;

    public uint Interval;
    public List<TorrentPeer> IPPairs;
    public uint Leechers;
    public uint Seeders;

    public AnnounceResponse(uint transaction, uint interval, uint leechers, uint seeders, List<TorrentPeer> ips)
    {
        Action = Action.Announce;
        TransactionID = transaction;

        Interval = interval;
        Leechers = leechers;
        Seeders = seeders;
        IPPairs = ips;

        var IPBytes = IPPairs.SelectMany(byteArr => byteArr.GetFormattedPair()).ToArray();

        Data = Pack.UInt32((uint)Action).Concat(Pack.UInt32(TransactionID)).Concat(Pack.UInt32(Interval))
            .Concat(Pack.UInt32(Leechers)).Concat(Pack.UInt32(Seeders)).Concat(IPBytes).ToArray();
    }

    public AnnounceResponse(uint transaction, uint interval, uint leechers, uint seeders, TorrentPeer ip)
    {
        Action = Action.Announce;
        TransactionID = transaction;

        Interval = interval;
        Leechers = leechers;
        Seeders = seeders;
        IPPairs = new List<TorrentPeer> { ip };

        var IPBytes = IPPairs.SelectMany(byteArr => byteArr.GetFormattedPair()).ToArray();

        Data = Pack.UInt32((uint)Action).Concat(Pack.UInt32(TransactionID)).Concat(Pack.UInt32(Interval))
            .Concat(Pack.UInt32(Leechers)).Concat(Pack.UInt32(Seeders)).Concat(IPBytes).ToArray();
    }
}