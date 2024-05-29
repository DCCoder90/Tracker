using System.Text;
using Tracker.Filesys.Bencode;

namespace Tracker.Filesys.Torrent;

internal class TorrentBencodeAdapter
{
    private static readonly IEnumerable<char> InvalidPathChars = Path.GetInvalidPathChars();

    public static void FillInfoFromFile(DictionaryDataType rootNode, TorrentFile file)
    {
        var meta = file.MetaInfo;

        var encoding = GetEncoding(rootNode);
        if (encoding != null)
        {
            file.Encoding = encoding;
            rootNode.TextEncoding = encoding;
        }

        file.Announce = GetValue(rootNode, "announce");
        var alist = rootNode["announce-list"] as ListDataType;
        file.AnnounceList = alist == null
            ? new List<string>()
            : alist.SelectMany(s =>
                    s is ListDataType
                        ? (s as ListDataType).Select(x => x.ToString()).ToArray()
                        : new[] { s.ToString() })
                .ToList();
        file.CreatedBy = GetValue(rootNode, "created by");
        file.Comment = GetValue(rootNode, "comment");
        file.CreationDate = null;
        var dateNode = rootNode["creation date"];
        if (dateNode != null && dateNode is IntegerDataType)
            file.CreationDate = new DateTime(1970, 1, 1).AddSeconds((dateNode as IntegerDataType).Value);

        file.Nodes = GetNodeList(rootNode, "nodes");

        file.IsPrivate = GetInt32(rootNode, "private") > 0;
        meta.Roothash = GetByteValue(rootNode, "root hash");
        meta.Filehash = GetByteValue(rootNode, "filehash");
        meta.Ed2k = GetByteValue(rootNode, "ed2k");
        meta.Md5Sum = GetByteValue(rootNode, "md5sum");

        var metaNode = meta.OriginalDataFragment = rootNode["info"] as DictionaryDataType;
        meta.Name = GetValue(metaNode, "name");
        meta.PieceLength = GetInt32(metaNode, "piece length");
        meta.Length = GetInt64(metaNode, "length");


        var filesNode = metaNode["files"] as ListDataType;
        if (filesNode != null)
            meta.Files = GetFileList(filesNode);


        meta.Pieces = GetByteValue(metaNode, "pieces");
        meta.Publisher = GetValue(metaNode, "publisher");
        meta.PublisherUrl = GetValue(metaNode, "publisher-url");
    }

    private static string GetValue(DataTypeBase node)
    {
        if (node is ByteStringDataType)
            return node.ToString();
        if (node is ListDataType)
        {
            var list = node as ListDataType;
            return string.Join(@"\", list.Select(s => s.ToString()));
        }

        return string.Empty;
    }

    private static string GetValue(DictionaryDataType dic, string key)
    {
        var node = dic[key + ".utf-8"];
        if (node != null)
        {
            node.TextEncoding = Encoding.UTF8;
            return GetValue(node);
        }

        node = dic[key];
        if (node == null)
            return string.Empty;

        return GetValue(node);
    }

    private static int GetInt32(DictionaryDataType dic, string key)
    {
        return (int)GetInt64(dic, key);
    }

    private static long GetInt64(DictionaryDataType dic, string key)
    {
        var node = dic[key];
        if (node == null || !(node is IntegerDataType))
            return 0;

        return (node as IntegerDataType).Value;
    }

    private static byte[] GetByteValue(DictionaryDataType dic, string key)
    {
        var node = dic[key];
        if (node == null || !(node is ByteStringDataType))
            return null;

        return node.Data;
    }

    private static string RemoveInvalidPathChars(string path)
    {
        return string.Join("", path.Where(s => !InvalidPathChars.Contains(s)));
    }

    private static List<FileItem> GetFileList(ListDataType list)
    {
        return list.Cast<DictionaryDataType>()
            .Select(s => new FileItem(GetValue(s, "name"), GetInt64(s, "length"))
            {
                Md5Sum = GetByteValue(s, "md5sum"),
                Path = RemoveInvalidPathChars(GetValue(s, "path")),
                TextEncoding = list.TextEncoding,
                Filehash = GetByteValue(s, "filehash"),
                Ed2k = GetByteValue(s, "ed2k")
            }).ToList();
    }

    private static List<string> GetValueList(DictionaryDataType dic, string key)
    {
        var node = dic[key];
        if (node == null || !(node is ListDataType))
            return new List<string>();

        return (node as ListDataType).Select(s => s.ToString()).ToList();
    }

    private static Encoding GetEncoding(DictionaryDataType rootNode)
    {
        var node = rootNode["encoding"];
        if (node != null)
        {
            var codeName = node.ToString();
            if (string.Compare(codeName, "UTF8", StringComparison.OrdinalIgnoreCase) == 0) return Encoding.UTF8;

            try
            {
                return Encoding.GetEncoding(node.ToString());
            }
            catch (Exception)
            {
                return Encoding.UTF8;
            }
        }

        node = rootNode["codepage"];
        if (node != null) return Encoding.GetEncoding((int)(node as IntegerDataType).Value);

        return Encoding.UTF8;
    }

    private static List<NodeEntry> GetNodeList(DictionaryDataType dic, string key)
    {
        var node = dic[key];
        if (node == null || !(node is ListDataType))
            return new List<NodeEntry>();

        return (node as ListDataType).Cast<ListDataType>().Select(s => new NodeEntry
        {
            Host = s.Value[0].ToString(),
            Port = (int)(s.Value[1] as IntegerDataType).Value
        }).ToList();
    }
}