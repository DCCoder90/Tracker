namespace Tracker.Net.Packets;

public abstract class Packet
{
    public Action Action;
    public uint TransactionID;
}