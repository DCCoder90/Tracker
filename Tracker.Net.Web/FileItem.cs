namespace Tracker.Net.Web;

public class FileItem
{
    public string Name { get; set; }

    public bool IsPaddingFile { get; set; }

    public string Path { get; set; }

    public long Length { get; set; }

    public string Md5Sum { get; set; }

    public string Filehash { get; set; }

    public string Ed2k { get; set; }

    public string TextEncoding { get; set; }
}