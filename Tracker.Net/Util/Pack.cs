namespace Tracker.Data.Util;

public static class Pack
{
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

    public static byte[] Int16(short i, Endianness e = Endianness.Big)
    {
        var bytes = BitConverter.GetBytes(i);

        if (NeedsFlipping(e)) Array.Reverse(bytes);

        return bytes;
    }

    public static byte[] Int32(int i, Endianness e = Endianness.Big)
    {
        var bytes = BitConverter.GetBytes(i);

        if (NeedsFlipping(e)) Array.Reverse(bytes);

        return bytes;
    }

    public static byte[] Int64(long i, Endianness e = Endianness.Big)
    {
        var bytes = BitConverter.GetBytes(i);

        if (NeedsFlipping(e)) Array.Reverse(bytes);

        return bytes;
    }

    public static byte[] UInt16(ushort i, Endianness e = Endianness.Big)
    {
        var bytes = BitConverter.GetBytes(i);

        if (NeedsFlipping(e)) Array.Reverse(bytes);

        return bytes;
    }

    public static byte[] UInt32(uint i, Endianness e = Endianness.Big)
    {
        var bytes = BitConverter.GetBytes(i);

        if (NeedsFlipping(e)) Array.Reverse(bytes);

        return bytes;
    }

    public static byte[] UInt64(ulong i, Endianness e = Endianness.Big)
    {
        var bytes = BitConverter.GetBytes(i);

        if (NeedsFlipping(e)) Array.Reverse(bytes);

        return bytes;
    }

    public static byte[] Float(float f, Endianness e = Endianness.Big)
    {
        return BitConverter.GetBytes(f);
    }

    public static byte[] Double(double f, Endianness e = Endianness.Big)
    {
        return BitConverter.GetBytes(f);
    }

    public static byte[] Hex(string str, Endianness e = Endianness.Big)
    {
        if (str.Length % 2 == 1) str += '0';

        var bytes = new byte[str.Length / 2];
        for (var i = 0; i < str.Length; i += 2)
            bytes[i / 2] = Convert.ToByte(str.Substring(NeedsFlipping(e) ? str.Length - i * 2 - 2 : i, 2), 16);

        return bytes;
    }
}