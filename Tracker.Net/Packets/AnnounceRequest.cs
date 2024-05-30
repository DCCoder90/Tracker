using Tracker.Net.Util;

namespace Tracker.Net.Packets;

public class AnnounceRequest : Packet
{
    public ulong ConnectionID;
    public ulong Downloaded;
    public byte[] InfoHash = new byte[20];
    public uint IpAddress;
    public ulong Key;
    public ulong Left;
    public uint NumWanted;
    public byte[] PeerID = new byte[20];
    public ushort Port;
    public uint TorrentEvent;
    public ulong Uploaded;


    public AnnounceRequest(byte[] data)
    {
        ConnectionID = Unpack.UInt64(data, 0);

        Action = (Action)Unpack.UInt32(data, 8);
        TransactionID = Unpack.UInt32(data, 12);

        InfoHash = data.GetBytes(16, 20);
        PeerID = data.GetBytes(36, 20);
        Downloaded = Unpack.UInt64(data, 56);
        Left = Unpack.UInt64(data, 64);
        Uploaded = Unpack.UInt64(data, 72);
        TorrentEvent = Unpack.UInt32(data, 80);
        IpAddress = Unpack.UInt32(data, 84);
        Key = Unpack.UInt64(data, 88);
        NumWanted = Unpack.UInt32(data, 92);
        Port = Unpack.UInt16(data, 96);
    }
}