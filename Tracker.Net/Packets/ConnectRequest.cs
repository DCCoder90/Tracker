using Tracker.Net.Util;

namespace Tracker.Net.Packets;

public class ConnectRequest : Packet
{
    public ulong ConnectionID;

    public ConnectRequest(byte[] response)
    {
        ConnectionID = Unpack.UInt64(response, 0);
        Action = (Action)Unpack.UInt32(response, 8);
        TransactionID = Unpack.UInt32(response, 12);
    }
}