namespace Tracker.Data.Util;

public static class Unpack
{
    public static short Int16(byte[] bytes, int start, Endianness e = Endianness.Big)
    {
        var intBytes = bytes.GetBytes(start, 2);

        if (NeedsFlipping(e)) Array.Reverse(intBytes);

        return BitConverter.ToInt16(intBytes, 0);
    }

    public static int Int32(byte[] bytes, int start, Endianness e = Endianness.Big)
    {
        var intBytes = bytes.GetBytes(start, 4);

        if (NeedsFlipping(e)) Array.Reverse(intBytes);

        return BitConverter.ToInt32(intBytes, 0);
    }

    public static long Int64(byte[] bytes, int start, Endianness e = Endianness.Big)
    {
        var intBytes = bytes.GetBytes(start, 8);

        if (NeedsFlipping(e)) Array.Reverse(intBytes);

        return BitConverter.ToInt64(intBytes, 0);
    }

    public static ushort UInt16(byte[] bytes, int start, Endianness e = Endianness.Big)
    {
        var intBytes = bytes.GetBytes(start, 2);

        if (NeedsFlipping(e)) Array.Reverse(intBytes);

        return BitConverter.ToUInt16(intBytes, 0);
    }

    public static uint UInt32(byte[] bytes, int start, Endianness e = Endianness.Big)
    {
        var intBytes = bytes.GetBytes(start, 4);

        if (NeedsFlipping(e)) Array.Reverse(intBytes);

        return BitConverter.ToUInt32(intBytes, 0);
    }

    public static ulong UInt64(byte[] bytes, int start, Endianness e = Endianness.Big)
    {
        var intBytes = bytes.GetBytes(start, 8);

        if (NeedsFlipping(e)) Array.Reverse(intBytes);

        return BitConverter.ToUInt64(intBytes, 0);
    }

    private static bool NeedsFlipping(Endianness e)
    {
        switch (e)
        {
            case Endianness.Big:
                return BitConverter.IsLittleEndian;
            case Endianness.Little:
                return !BitConverter.IsLittleEndian;
        }

        return false;
    }

    public static string Hex(byte[] bytes, Endianness e = Endianness.Big)
    {
        var str = "";

        foreach (var b in bytes) str += string.Format("{0:X2}", b);

        return str;
    }
}