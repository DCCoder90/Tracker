namespace Tracker.TorrentFile.Bencode;

public class IntegerDataType : DataTypeBase<long>
{
    public IntegerDataType(long value)
        : base(value)
    {
    }

    public IntegerDataType()
    {
    }

    public override Type MathedClrType => Value > int.MaxValue ? typeof(long) : typeof(int);

    public override string ToString()
    {
        return Value.ToString();
    }

    #region Overrides of DataTypeBase<long>

    protected override void WriteTo(Stream stream)
    {
        stream.WriteByte((byte)'i');
        base.WriteTo(stream);
        stream.WriteByte((byte)'e');
    }

    protected override void SynchorizeData(bool fromDataToValue)
    {
        if (fromDataToValue) _value = long.Parse(new string(Array.ConvertAll(_data, s => (char)s)));
        else _data = Array.ConvertAll(_value.ToString().ToCharArray(), s => (byte)s);
    }

    public override int DataSize => base.DataSize + 2;

    #endregion
}