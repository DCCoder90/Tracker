namespace Tracker.Net.Web;

public class Torrent
{
    public string Name { get; set; }
    public DateTime? CreationDate { get; set; }
    public string Comment { get; set; }
    public string CreatedBy { get; set; }
    public string Encoding { get; set; }
    public bool IsPrivate { get; set; }
    public string MetaInfoHashString { get; set; }
    public IEnumerable<FileItem> Files { get; set; }
    
    public int PieceLength { get; set; }
    public string Pieces { get; set; }
    public string Roothash { get; set; }
    public string Publisher { get; set; }
    public string PublisherUrl { get; set; }
    public string Md5Sum { get; set; }
    public string Filehash { get; set; }
    public string Ed2k { get; set; }
}