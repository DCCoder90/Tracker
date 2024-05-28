using System.Security.Cryptography;
using System.Text;
using Tracker.TorrentFile.Bencode;

namespace Tracker.TorrentFile.Torrent;

public class TorrentFile
{
    public TorrentFile()
    {
        AnnounceList = new List<string>();
        Encoding = Encoding.UTF8;
        MetaInfo = new MetaInfo();
    }

    public TorrentFile(string path, LoadFlag flag = LoadFlag.None)
        : this()
    {
        Load(path, flag);
    }

    public string Path { get; set; }

    public MetaInfo MetaInfo { get; }

    public string Announce { get; set; }

    public List<string> AnnounceList { get; set; }

    public DateTime? CreationDate { get; set; }

    public string Comment { get; set; }

    public string CreatedBy { get; set; }

    public Encoding Encoding { get; set; }

    public List<NodeEntry> Nodes { get; set; }

    public List<string> HttpSeeds { get; set; }

    public bool IsPrivate { get; set; }

    public byte[] MetaInfoHash { get; private set; }

    public string MetaInfoHashString { get; private set; }

    public List<FileItem> Files => MetaInfo.Files;

    public string Name
    {
        get => MetaInfo.Name;
        set => MetaInfo.Name = value;
    }

    public void Load(string path, LoadFlag flag = LoadFlag.None)
    {
        Path = path;
        using (var fs = new FileStream(path, FileMode.Open))
        {
            Load(fs, flag);
        }
    }

    public void Load(Stream stream, LoadFlag flag = LoadFlag.None)
    {
        var tor = BencodeParser.Parse(Encoding.UTF8, stream);
        TorrentBencodeAdapter.FillInfoFromFile(tor[0] as DictionaryDataType, this);

        if ((LoadFlag.LoadInfoSectionData & flag) == LoadFlag.LoadInfoSectionData)
        {
            if (!stream.CanSeek) throw new InvalidOperationException("stream can not be seeked.");

            var data = MetaInfo.OriginalDataFragment;
            MetaInfo.OriginalDataFragmentBuffer = new byte[(int)(data.DataEndPosition - data.DataStartPosition) + 1];

            stream.Seek(data.DataStartPosition, SeekOrigin.Begin);
            stream.Read(MetaInfo.OriginalDataFragmentBuffer, 0, MetaInfo.OriginalDataFragmentBuffer.Length);
        }

        if (flag.HasFlag(LoadFlag.ComputeMetaInfoHash))
        {
            var data = MetaInfo.OriginalDataFragment;
            var buffer = MetaInfo.OriginalDataFragmentBuffer;
            if (buffer == null)
            {
                if (!stream.CanSeek) throw new InvalidOperationException("stream can not be seeked.");

                buffer = new byte[(int)(data.DataEndPosition - data.DataStartPosition) + 1];
                stream.Seek(data.DataStartPosition, SeekOrigin.Begin);
                stream.Read(buffer, 0, buffer.Length);
            }

            var sha = SHA1.Create();
            MetaInfoHash = sha.ComputeHash(buffer);
            MetaInfoHashString = BitConverter.ToString(MetaInfoHash).Replace("-", "").ToUpper();
        }
    }
}