using System.Collections;
using System.Text;

namespace Tracker.TorrentFile.UriProtocol;

public abstract class ProtocolBase
{
    private string _url;


    public ProtocolBase(string procotolName)
        : this()
    {
        ProtocolName = procotolName;
    }


    public ProtocolBase()
    {
        Properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        UrlParameterSeperator = '&';
    }

    public char UrlParameterSeperator { get; set; }


    public string ProtocolName { get; protected set; }


    public Dictionary<string, object> Properties { get; }


    public string Url
    {
        get
        {
            if (string.IsNullOrEmpty(_url))
                _url = GenerateUrl();
            return _url;
        }
        set
        {
            if (value == _url)
                return;

            _url = null;
            Parse(value);
        }
    }

    public void SetProperty(string key, object data)
    {
        Properties[key] = data;
    }

    public T GetProperty<T>(string key)
    {
        return (T)Properties[key];
    }

    protected virtual void Parse(string uri)
    {
        if (string.IsNullOrEmpty(uri)) return;

        var index = uri.IndexOf("://");
        if (index <= 0) return;

        var tagName = uri.Substring(0, index);
        if (!string.IsNullOrEmpty(tagName) && string.Compare(tagName, ProtocolName, true) != 0)
            throw new InvalidProgramException("URI Protocol Name Conflicted.");


        ProtocolName = tagName;
        var body = uri.Substring(index + 3);
        ParseUriBody(body);
    }

    protected virtual void ParseUriBody(string bodyUri)
    {
        var fragments = bodyUri.Split(UrlParameterSeperator);
        foreach (var fragment in fragments) ParseUriDataFragment(fragment);
    }

    protected virtual void ParseUriDataFragment(string data)
    {
        if (string.IsNullOrEmpty(data))
            return;

        var index = data.IndexOf('=');
        if (index == -1)
            return;

        var key = data.Substring(0, index);
        var value = data.Substring(index + 1);

        var obj = ParseUriDataArea(key, value);
        if (obj != null)
        {
            if (Properties.ContainsKey(key))
            {
                var container = Properties[key];
                if (container is ArrayList)
                {
                    (container as ArrayList).Add(obj);
                }
                else
                {
                    container = new ArrayList
                    {
                        container,
                        obj
                    };
                    Properties[key] = container;
                }
            }
            else
            {
                Properties.Add(key, obj);
            }
        }
    }

    protected virtual object ParseUriDataArea(string key, string data)
    {
        return data;
    }

    public string GenerateUrl()
    {
        var sb = new StringBuilder(0x400);
        GenerateUrl(sb);
        return sb.ToString();
    }

    protected virtual void GenerateUrl(StringBuilder sb)
    {
        sb.Append(ProtocolName.ToLower());
        sb.Append("://");
    }

    protected virtual void GenerateUrlBody(StringBuilder sb)
    {
        var allProps = Properties.ToArray();
        for (var i = 0; i < allProps.Length; i++)
        {
            var property = allProps[i];

            GenerateUrlParameter(property.Key, property.Value, sb);
            if (i < allProps.Length - 1)
                sb.Append(UrlParameterSeperator);
        }
    }

    protected virtual void GenerateUrlParameter(string key, object data, StringBuilder sb)
    {
        if (data == null)
        {
            GenerateUrlParameterUnit(key, null, sb);
            return;
        }

        if (data is IDictionary)
        {
            var col = data as IDictionary;
            var count = col.Count;
            var index = 0;
            foreach (var c in col.Values)
            {
                GenerateUrlParameterUnit(key, c, sb);
                if (index++ < count - 1)
                    sb.Append(UrlParameterSeperator);
            }

            return;
        }


        if (data is ICollection)
        {
            var col = data as ICollection;
            var count = col.Count;
            var index = 0;
            foreach (var c in col)
            {
                GenerateUrlParameterUnit(key, c, sb);
                if (index++ < count - 1)
                    sb.Append(UrlParameterSeperator);
            }

            return;
        }

        GenerateUrlParameterUnit(key, data, sb);
    }

    protected virtual void GenerateUrlParameterUnit(string key, object data, StringBuilder sb)
    {
        if (data == null)
            sb.Append(key + "=");
        else
            sb.Append(key + "=" + data);
    }

    public override string ToString()
    {
        return Url;
    }
}