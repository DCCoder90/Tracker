using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Tracker.Filesys.UriProtocol;

public class MagnetProtocol : ProtocolBase
{
    public MagnetProtocol()
        : base("magnet")
    {
    }

    public MagnetProtocol(string url)
        : base("magnet")
    {
        Url = url;
    }

    public string DownloadName
    {
        get => HttpUtility.UrlDecode((string)Properties["dn"] ?? "");
        set
        {
            if (Properties.ContainsKey("dn"))
                Properties["dn"] = HttpUtility.UrlEncode(value);
            else
                Properties.Add("dn", HttpUtility.UrlEncode(value));
        }
    }

    public Dictionary<MagnetProtocolXtType, MagnetProtocolXtBase> Hashes
    {
        get
        {
            var dic = (Dictionary<MagnetProtocolXtType, MagnetProtocolXtBase>)Properties["xt"];
            if (dic == null)
            {
                dic = new Dictionary<MagnetProtocolXtType, MagnetProtocolXtBase>();
                Properties.Add("xt", dic);
            }

            return dic;
        }
    }

    public long? DlSize
    {
        get => Properties.ContainsKey("xl") ? (long?)Properties["xl"] : null;
        set
        {
            if (value == null)
            {
                if (Properties.ContainsKey("xl")) Properties.Remove("xl");
            }
            else
            {
                Properties["xl"] = value.ToString();
            }
        }
    }

    protected override void Parse(string uri)
    {
        if (!uri.StartsWith("magnet:?"))
            return;

        uri = uri.Remove(0, 8);
        ParseUriBody(uri);
    }

    protected override object ParseUriDataArea(string key, string data)
    {
        if (key == "xt" || key.StartsWith("xt."))
        {
            var m = Regex.Match(data, @"urn:([^:]+|tree:tiger):([^:]+)");
            if (!m.Success) return null;

            var type = m.Groups[1].Value;
            var hash = m.Groups[2].Value;

            MagnetProtocolXtBase xtItem = null;
            switch (type)
            {
                case "bitprint":
                    var hashes = hash.Split('.');
                    if (hashes.Length == 2)
                        xtItem = new MagnetProtocolBitPrint(hashes[0], hashes[1]);
                    break;
                case "tth:tiger":
                    xtItem = new MagnetProtocolXtTth(hash);
                    break;
                default:
                    try
                    {
                        var xt = (MagnetProtocolXtType)Enum.Parse(typeof(MagnetProtocolXtType), type, true);
                        xtItem = new MagnetProtocolXtHashedBase(xt, hash);
                    }
                    catch (Exception)
                    {
                        xtItem = null;
                    }

                    break;
            }

            if (xtItem == null)
                return null;

            var dict = (Dictionary<MagnetProtocolXtType, MagnetProtocolXtBase>)Properties["xt"];
            dict[xtItem.Type] = xtItem;

            return null;
        }

        if (key == "xl") return long.Parse(data);

        return base.ParseUriDataArea(key, data);
    }

    protected override void GenerateUrl(StringBuilder sb)
    {
        sb.Append("magnet:?");
        GenerateUrlBody(sb);
    }
}