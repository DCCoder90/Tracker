using System.Text;

namespace Tracker.Filesys.Bencode;

public class ByteStringDataType : DataTypeBase<string>
{
    public ByteStringDataType(string value)
        : base(value)
    {
    }

    public ByteStringDataType()
    {
    }


    public override string ToString()
    {
        return base.Value;
    }

    #region Overrides of DataTypeBase<string>

    protected override void WriteTo(Stream stream)
    {
        throw new NotImplementedException();
    }

    public override int DataSize
    {
        get
        {
            var datasize = base.DataSize;
            return datasize + 1 + datasize.ToString().Length;
        }
    }

    protected override void SynchorizeData(bool fromDataToValue)
    {
        if (fromDataToValue)
        {
            if (_data == null || _data.Length == 0)
                _value = string.Empty;
            else
                _value = TextEncoding.GetString(_data);
        }
        else
        {
            if (string.IsNullOrEmpty(_value))
                _data = new byte[]
                {
                };
            else
                _data = TextEncoding.GetBytes(_value);
        }
    }

    public override Encoding TextEncoding
    {
        get => base.TextEncoding;
        set
        {
            value = value ?? Encoding.UTF8;
            if (value == base.TextEncoding)
                return;

            base.TextEncoding = value;
            _value = base.TextEncoding.GetString(Data);
        }
    }

    #endregion
}

public class ByteStringDataTypeIgnoreCaseComparer : IComparer<ByteStringDataType>, IEqualityComparer<ByteStringDataType>
{
    #region IComparer<ByteStringDataType> 成员

    public int Compare(ByteStringDataType x, ByteStringDataType y)
    {
        if ((x == null) ^ (y == null))
            return x == null ? -1 : 1;
        if (x == null)
            return 0;

        return StringComparer.OrdinalIgnoreCase.Compare(x.Value, y.Value);
    }

    #endregion

    public bool Equals(ByteStringDataType x, ByteStringDataType y)
    {
        if ((x == null) ^ (y == null))
            return false;

        return x == null || StringComparer.OrdinalIgnoreCase.Equals(x.Value, y.Value);
    }

    public int GetHashCode(ByteStringDataType obj)
    {
        return obj == null ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Value);
    }
}