using System.Text;
using System.Web;

namespace Tracker.TorrentFile.UriProtocol;

public class Ed2kProtocol : ProtocolBase
{
    public enum LinkType
    {
        Unknown,
        File,
        Server,
        ServerList,
        NodesList
    }

    public Ed2kProtocol()
        : base("ed2k")
    {
    }

    public Ed2kProtocol(string url)
        : base("ed2k")
    {
        Url = url;
    }

    public bool IsValid
    {
        get
        {
            if (UrlType == LinkType.File)
                return Properties.ContainsKey("name")
                       && Properties.ContainsKey("size")
                       && Properties.ContainsKey("hash");

            return false;
        }
    }

    public LinkType UrlType
    {
        get
        {
            switch (TypeString)
            {
                case "file":
                    return LinkType.File;
                case "server":
                    return LinkType.Server;
                case "serverlist":
                    return LinkType.ServerList;
                case "nodeslist":
                    return LinkType.NodesList;
            }

            return LinkType.Unknown;
        }
    }

    protected override void GenerateUrl(StringBuilder sb)
    {
        base.GenerateUrl(sb);

        var list = new List<string>(10) { "", TypeString };

        if (UrlType == LinkType.File)
        {
            list.Add(FileName);
            list.Add(Size.ToString());
            list.Add(FileHash);

            if (!string.IsNullOrEmpty(RootHash)) list.Add("h=" + RootHash);
            if (HashSet.Count > 0) list.Add("p=" + string.Join(":", HashSet.ToArray()));
            if (WebSources.Count > 0) WebSources.ForEach(s => list.Add("s=" + s));
            if (Sources.Count > 0)
            {
                list.Add("/");
                list.Add("sources," + string.Join(",", Sources.Select(s => s.Host + ":" + s.Port).ToArray()));
            }
        }
        else if (UrlType == LinkType.NodesList || UrlType == LinkType.ServerList)
        {
            list.Add(SourceUrl);
        }
        else if (UrlType == LinkType.Server)
        {
            var host = ServerAddress;
            list.Add(host.Host);
            list.Add(host.Port.ToString());
        }

        sb.Append(string.Join("|", list.ToArray()));
        sb.Append("/");
    }

    protected override void ParseUriBody(string bodyUri)
    {
        base.ParseUriBody(bodyUri);

        var segments = bodyUri.Trim('/').Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        var parseIndex = 1;

        TypeString = segments[0];
        if (segments[0] == "file")
        {
            FileName = segments[parseIndex++];
            Size = int.Parse(segments[parseIndex++]);
            FileHash = segments[parseIndex++];
        }
        else if (segments[0] == "serverlist")
        {
            SourceUrl = segments[parseIndex++];
        }
        else if (segments[0] == "nodeslist")
        {
            SourceUrl = segments[parseIndex++];
        }
        else if (segments[0] == "server")
        {
            ServerAddress = new DnsEndPoint(segments[parseIndex++], int.Parse(segments[parseIndex++]));
        }

        for (var i = parseIndex; i < segments.Length; i++)
        {
            var segment = segments[i];

            if (segment.StartsWith("s="))
                WebSources.Add(HttpUtility.UrlDecode(segment.Remove(0, 2), Encoding.UTF8));
            else if (segment.StartsWith("sources,"))
                Sources.AddRange(segment.Remove(0, 8).Split(',').Select(s =>
                {
                    var arg = s.Split(':');
                    return new DnsEndPoint(arg[0], int.Parse(arg[1]));
                }).ToArray());
            else if (segment.StartsWith("h="))
                RootHash = segment.Remove(0, 2);
            else if (segment.StartsWith("p=")) HashSet.AddRange(segment.Remove(0, 2).Split(':'));
        }
    }

    public string TypeString
    {
        get => GetProperty<string>("type");
        set => SetProperty("type", value);
    }

    public string FileName
    {
        get => GetProperty<string>("name");
        set => SetProperty("name", HttpUtility.UrlDecode(value, Encoding.UTF8));
    }

    public long Size
    {
        get => GetProperty<long>("size");
        set => SetProperty("size", value);
    }

    public string FileHash
    {
        get => GetProperty<string>("hash");
        set => SetProperty("hash", value);
    }


    public string RootHash
    {
        get => GetProperty<string>("roothash");
        set => SetProperty("roothash", value);
    }

    public List<string> HashSet
    {
        get
        {
            var list = GetProperty<List<string>>("hashset");
            if (list == null)
            {
                list = new List<string>();
                HashSet = list;
            }

            return list;
        }
        private set => SetProperty("hashset", value);
    }

    public List<DnsEndPoint> Sources
    {
        get
        {
            var list = GetProperty<List<DnsEndPoint>>("sources");
            if (list == null)
            {
                list = new List<DnsEndPoint>();
                Sources = list;
            }

            return list;
        }
        private set => SetProperty("sources", value);
    }

    public string SourceUrl
    {
        get => GetProperty<string>("sourceurl");
        set => SetProperty("sourceurl", value);
    }

    public List<string> WebSources
    {
        get
        {
            var list = GetProperty<List<string>>("websources");
            if (list == null)
            {
                list = new List<string>();
                WebSources = list;
            }

            return list;
        }
        set => SetProperty("websources", value);
    }

    public DnsEndPoint ServerAddress
    {
        get => GetProperty<DnsEndPoint>("address");
        set => SetProperty("address", value);
    }
}

#if !NET4

public class DnsEndPoint
{
    public DnsEndPoint(string hostAddress, int port)
    {
        Host = hostAddress;
        Port = port;
    }

    public DnsEndPoint()
    {
    }

    public string Host { get; set; }

    public int Port { get; set; }
}

#endif