using System.Net;
using Tracker.Data.Util;

namespace Tracker.Data.Torrent;

public class TorrentPeer
{
    public byte[] IP;
    public ushort Port;

    public TorrentPeer(uint ip, ushort port)
    {
        IP = Pack.UInt32(ip);
        Port = port;
    }

    public TorrentPeer(string ip, ushort port)
    {
        IP = IPAddress.Parse(ip).GetAddressBytes();
        Port = port;
    }

    public TorrentPeer(string ip, int port)
    {
        IP = IPAddress.Parse(ip).GetAddressBytes();
        Port = (ushort)port;
    }

    public TorrentPeer(IPAddress ip, ushort port)
    {
        IP = ip.GetAddressBytes();
        Port = port;
    }

    public TorrentPeer(string redisResponse)
    {
        var parts = redisResponse.Split(':');
        IP = IPAddress.Parse(parts[0]).GetAddressBytes();
        Port = ushort.Parse(parts[1]);
    }

    public TorrentPeer(byte[] peer)
    {
        IP = peer.GetBytes(0, 4);
        Port = Unpack.UInt16(peer, 4);
    }

    public string StringPeer()
    {
        return GetIPString() + ":" + Port;
    }

    public byte[] GetFormattedPair()
    {
        return IP.Cat(Pack.UInt16(Port));
    }

    public string GetIPString()
    {
        return new IPAddress(IP).ToString();
    }
}