namespace Tracker.Net.Util;

public static class UtilityFunctions
{
    public static bool GetBit(this byte t, ushort n)
    {
        return (t & (1 << n)) != 0;
    }

    public static byte SetBit(this byte t, ushort n)
    {
        return (byte)(t | (1 << n));
    }

    public static byte[] GetBytes(this byte[] bytes, int start, int length = -1)
    {
        var l = length;
        if (l == -1) l = bytes.Length - start;

        var intBytes = new byte[l];

        for (var i = 0; i < l; i++) intBytes[i] = bytes[start + i];

        return intBytes;
    }

    public static byte[] Cat(this byte[] first, byte[] second)
    {
        var returnBytes = new byte[first.Length + second.Length];

        first.CopyTo(returnBytes, 0);
        second.CopyTo(returnBytes, first.Length);

        return returnBytes;
    }
}