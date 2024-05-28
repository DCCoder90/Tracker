using System.Text;

namespace Tracker.TorrentFile.Bencode;

public static class Utility
{
    public static ByteStringDataType Create(Encoding encoding, string value)
    {
        var node = new ByteStringDataType
        {
            TextEncoding = encoding
        };
        node.Value = value;

        return node;
    }

    public static void AddToDictionary(DictionaryDataType dict, string key, string value)
    {
        AddToDictionary(dict, dict.TextEncoding ?? Encoding.Default, key, value);
    }
    
    public static void AddToDictionary(DictionaryDataType dict, string key, int value)
    {
        dict.Add(Create(dict.TextEncoding, key), new IntegerDataType(value));
    }
    
    public static void AddToDictionary(DictionaryDataType dict, string key, long value)
    {
        dict.Add(Create(dict.TextEncoding, key), new IntegerDataType(value));
    }
    
    public static void AddToDictionary(DictionaryDataType dict, Encoding encoding, string key, string value)
    {
        dict.Add(Create(encoding, key), Create(encoding, value));
    }
}