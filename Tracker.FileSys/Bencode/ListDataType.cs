using System.Collections;
using System.Text;

namespace Tracker.TorrentFile.Bencode;

public class ListDataType : DataTypeBase<List<DataTypeBase>>, IEnumerable<DataTypeBase>
{
    public ListDataType()
    {
        Value = new List<DataTypeBase>();
    }

    #region IEnumerable<DataTypeBase> 成员

    public IEnumerator<DataTypeBase> GetEnumerator()
    {
        return Value.GetEnumerator();
    }

    #endregion

    #region IEnumerable 成员

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Value.GetEnumerator();
    }

    #endregion

    public void Add(DataTypeBase data)
    {
        Value.Add(data);
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
            base.TextEncoding = value;
            Value.ForEach(s => s.TextEncoding = value);
        }
    }

    #endregion
}