using System.Text;

namespace Tracker.TorrentFile.Bencode;

public class BencodeParser
{
    public static List<DataTypeBase> Parse(Encoding textEncoding, Stream stream)
    {
        if (textEncoding == null)
            throw new ArgumentNullException("textEncoding");
        if (stream == null)
            throw new ArgumentNullException("stream");
        var result = new List<DataTypeBase>();

        AddListContentToList(textEncoding, stream, result);

        return result;
    }

    private static void AddListContentToList(Encoding textEncoding, Stream stream, List<DataTypeBase> list)
    {
        if (textEncoding == null)
            throw new ArgumentNullException("textEncoding");
        if (stream == null)
            throw new ArgumentNullException("stream");
        if (list == null)
            throw new ArgumentNullException("list");


        int ch;
        while ((ch = stream.ReadByte()) != -1 && ch != 'e')
        {
            stream.Seek(-1, SeekOrigin.Current);

            if (ch >= '0' && ch <= '9')
                list.Add(ParseAsString(textEncoding, stream));
            else if (ch == 'i')
                list.Add(ParseAsNumberic(stream));
            else if (ch == 'l')
                list.Add(ParseAsList(textEncoding, stream));
            else if (ch == 'd')
                list.Add(ParseAsDicionary(textEncoding, stream));
            else
                break; 
        }
    }

    private static IntegerDataType ParseAsNumberic(Stream stream)
    {
        var start = stream.Position;

        if (stream.ReadByte() != 'i')
            throw new Exception("Expect for 'i'.");

        var buffer = new List<char>(10);
        int ch;
        while ((ch = stream.ReadByte()) != -1 && ch != 'e') buffer.Add((char)ch);
        if (ch != 'e')
            throw new UnexpectEndException();

        return new IntegerDataType(long.Parse(new string(buffer.ToArray())))
        {
            DataStartPosition = start,
            DataEndPosition = stream.Position - 1
        };
    }

    private static ByteStringDataType ParseAsString(Encoding textEncoding, Stream stream)
    {
        var start = stream.Position;

        var buffer = new List<char>(10);
        int ch;
        while ((ch = stream.ReadByte()) != -1 && ch != ':') buffer.Add((char)ch);
        if (ch != ':')
            throw new UnexpectEndException();

        var length = int.Parse(new string(buffer.ToArray()));
        if (length > 0x400000)
            throw new ArgumentOutOfRangeException();

        var buf = new byte[length];
        if (stream.Read(buf, 0, buf.Length) != buf.Length)
            throw new UnexpectEndException();

        return new ByteStringDataType
        {
            TextEncoding = textEncoding,
            Data = buf,
            DataStartPosition = start,
            DataEndPosition = stream.Position - 1
        };
    }

    private static ListDataType ParseAsList(Encoding textEncoding, Stream stream)
    {
        var start = stream.Position;

        var list = new ListDataType();

        if (stream.ReadByte() != 'l')
            throw new InvalidOperationException();
        AddListContentToList(textEncoding, stream, list.Value);

        list.DataStartPosition = start;
        list.DataEndPosition = stream.Position - 1;

        return list;
    }

    private static DictionaryDataType ParseAsDicionary(Encoding textEncoding, Stream stream)
    {
        var result = new DictionaryDataType
        {
            TextEncoding = textEncoding,
            DataStartPosition = stream.Position
        };

        if (stream.ReadByte() != 'd')
            throw new InvalidOperationException();

        int ch;
        while ((ch = stream.ReadByte()) != -1 && ch != 'e')
        {
            stream.Seek(-1, SeekOrigin.Current);
            var key = ParseAsString(textEncoding, stream);
            DataTypeBase value;
            ch = stream.ReadByte();
            if (ch == -1)
                throw new UnexpectEndException();

            stream.Seek(-1, SeekOrigin.Current);
            if (ch >= '0' && ch <= '9')
                value = ParseAsString(textEncoding, stream);
            else if (ch == 'i')
                value = ParseAsNumberic(stream);
            else if (ch == 'l')
                value = ParseAsList(textEncoding, stream);
            else if (ch == 'd')
                value = ParseAsDicionary(textEncoding, stream);
            else
                break;

            result.Add(key, value);
        }

        if (ch != 'e')
            throw new UnexpectEndException();

        result.DataEndPosition = stream.Position - 1;

        return result;
    }
}