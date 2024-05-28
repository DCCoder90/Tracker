using Tracker.Data.Util;

namespace Tracker.Data.Packets;

public class ConnectResponse : Packet
{
    public byte[] Data;

    public UInt64 ConnectionID;
    public ConnectResponse(Action action, UInt32 transaction, UInt64 connection)
    {
        Action = action;
        TransactionID = transaction;
        ConnectionID = connection;

        Data = Pack.UInt32((uint)Action).
            Concat(Pack.UInt32(TransactionID)).
            Concat(Pack.UInt64(ConnectionID)).ToArray();
    }
}