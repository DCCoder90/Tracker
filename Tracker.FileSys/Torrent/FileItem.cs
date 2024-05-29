using System.Text;
using Tracker.Filesys.Bencode;

namespace Tracker.Filesys.Torrent;

public class FileItem
{
    public FileItem()
    {
    }

    public FileItem(string path, long length)
    {
        Path = path;
        Length = length;
    }

    public string Md5SumString
    {
        get
        {
            if (Md5Sum == null)
                return string.Empty;
            return Encoding.ASCII.GetString(Md5Sum).ToUpper();
        }
    }

    public string Ed2khash
    {
        get
        {
            if (Ed2k == null)
                return string.Empty;
            return Encoding.ASCII.GetString(Ed2k).ToUpper();
        }
    }

    public string FilehashString
    {
        get
        {
            if (Filehash == null)
                return string.Empty;
            return Encoding.ASCII.GetString(Filehash).ToUpper();
        }
    }

    public string Name => System.IO.Path.GetFileName(Path);

    public bool IsPaddingFile => Name.IndexOf("_____padding_file_") != -1;

    public string Path { get; set; }

    public long Length { get; set; }

    public byte[] Md5Sum { get; set; }

    public byte[] Filehash { get; set; }

    public byte[] Ed2k { get; set; }

    public Encoding TextEncoding { get; set; }

    public static implicit operator FileItem(DictionaryDataType data)
    {
        var path = data["path"];
        var length = data["length"];

        if (path == null || length == null)
            return null;

        return new FileItem
        {
            Path = path.ToString(),
            Length = (length as IntegerDataType).Value
        };
        return null;
    }

    public static implicit operator DictionaryDataType(FileItem item)
    {
        var dic = new DictionaryDataType
        {
            TextEncoding = item.TextEncoding
        };
        var pathList = item.Path.Split(new[]
        {
            '/',
            '\\'
        }, StringSplitOptions.RemoveEmptyEntries);

        var plist = new ListDataType
        {
            TextEncoding = item.TextEncoding
        };

        Utility.AddToDictionary(dic, "path", item.Path);
        Utility.AddToDictionary(dic, "length", item.Length);

        return dic;
    }
}