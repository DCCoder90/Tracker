using Tracker.Data.Util;

namespace Tracker.Data.Packets;

public class ConnectResponse : Packet
{
    public ulong ConnectionID;
    public byte[] Data;

    public ConnectResponse(Action action, uint transaction, ulong connection)
    {
        Action = action;
        TransactionID = transaction;
        ConnectionID = connection;

        Data = Pack.UInt32((uint)Action).Concat(Pack.UInt32(TransactionID)).Concat(Pack.UInt64(ConnectionID)).ToArray();
    }
}