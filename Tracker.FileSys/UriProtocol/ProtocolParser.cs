namespace Tracker.TorrentFile.UriProtocol;

public class ProtocolParser
{
    public static ProtocolBase Parse(string url)
    {
        if (string.IsNullOrEmpty(url)) return null;

        var index = url.IndexOf("://");
        if (index <= 0) return null;

        var tagName = url.Substring(0, index);
        if (string.Compare(tagName, "ed2k", StringComparison.OrdinalIgnoreCase) == 0) return new Ed2kProtocol(url);

        return null;
    }
}