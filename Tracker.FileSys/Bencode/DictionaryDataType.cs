using System.Collections;
using System.Text;

namespace Tracker.TorrentFile.Bencode;

public class DictionaryDataType : DataTypeBase, IEnumerable<KeyValuePair<string, DataTypeBase>>
{
    private Dictionary<string, DataTypeBase> _keyMap;
    private readonly object _lockObject = new();
    private SortedDictionary<string, DataTypeBase> _valueMap;

    public DictionaryDataType()
    {
        _valueMap = new SortedDictionary<string, DataTypeBase>(StringComparer.OrdinalIgnoreCase);
        _keyMap = new Dictionary<string, DataTypeBase>(StringComparer.OrdinalIgnoreCase);
    }

    public DataTypeBase this[string key]
    {
        get
        {
            DataTypeBase value;

            _valueMap.TryGetValue(key, out value);
            return value;
        }
    }

    #region IEnumerable<KeyValuePair<string,DataTypeBase>> 成员

    public IEnumerator<KeyValuePair<string, DataTypeBase>> GetEnumerator()
    {
        return _valueMap.GetEnumerator();
    }

    #endregion

    #region IEnumerable 成员

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _valueMap.GetEnumerator();
    }

    #endregion

    public void Add(ByteStringDataType key, DataTypeBase value)
    {
        if (key == null) throw new ArgumentNullException("key");
        if (value == null) throw new ArgumentNullException("value");

        lock (_lockObject)
        {
            if (_valueMap.ContainsKey(key.Value))
            {
                var data = _valueMap[key.Value];
                if (value is ListDataType && data is ListDataType)
                {
                    var list = value as ListDataType;
                    var olist = data as ListDataType;
                    
                    foreach (var item in list)
                    {
                        olist.Add(item);
                    }
                }
            }
            else
            {
                _valueMap.Add(key.Value, value);
                _keyMap.Add(key.Value, key);
            }
        }
    }

    #region Overrides of DataTypeBase

    protected override void WriteTo(Stream stream)
    {
        throw new NotImplementedException();
    }

    protected override void SynchorizeData(bool fromDataToValue)
    {
    }

    public override Encoding TextEncoding
    {
        get => base.TextEncoding;
        set
        {
            if (base.TextEncoding == value)
                return;

            base.TextEncoding = value;

            lock (_lockObject)
            {
                var allkeys = _valueMap.Keys.ToArray();
                var map1 = _valueMap;
                var map2 = _keyMap;
                _valueMap = new SortedDictionary<string, DataTypeBase>();
                _keyMap = new Dictionary<string, DataTypeBase>();
                foreach (var key in allkeys)
                {
                    var v_key = map2[key];
                    var v_value = map1[key];

                    v_key.TextEncoding = value;
                    v_value.TextEncoding = value;

                    _valueMap.Add(v_key.ToString(), v_value);
                    _keyMap.Add(v_key.ToString(), v_key);
                }
            }
        }
    }

    #endregion
}