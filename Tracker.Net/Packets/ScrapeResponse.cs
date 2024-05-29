using Tracker.Data.Torrent;
using Tracker.Data.Util;

namespace Tracker.Data.Packets;

public class ScrapeResponse : Packet
{
    public byte[] Data;
    public List<TorrentInfo> ScrapeInfo;

    public ScrapeResponse(uint transactionID, List<TorrentInfo> scrapeInfo)
    {
        Action = Action.Scrape;
        TransactionID = transactionID;

        var ScrapeBytes = scrapeInfo.SelectMany(torrent => torrent.PackedTorrentInfo()).ToArray();

        Data = Pack.UInt32((uint)Action).Concat(Pack.UInt32((uint)Action)).Concat(Pack.UInt32(transactionID))
            .Concat(ScrapeBytes).ToArray();
    }
}