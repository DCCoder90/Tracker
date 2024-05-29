using Tracker.Data.Util;

namespace Tracker.Data.Packets;

public class ScrapeRequest : Packet
{
    public ulong ConnectionID;
    public List<byte[]> InfoHashes = new();


    public ScrapeRequest(byte[] data)
    {
        ConnectionID = Unpack.UInt64(data, 0);
        Action = (Action)Unpack.UInt32(data, 8);
        TransactionID = Unpack.UInt32(data, 12);

        var totalHashes = (data.Length - 16) / 20;
        for (var i = 0; i < totalHashes; i += 1)
        {
            var hash = data.GetBytes(16 + i * 20, 20);
            InfoHashes.Add(hash);
        }
    }
}