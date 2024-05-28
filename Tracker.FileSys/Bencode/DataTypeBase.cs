using System.Text;

namespace Tracker.TorrentFile.Bencode;

public abstract class DataTypeBase
{
    protected byte[] _data;

    private Encoding _textEncoding;

    public DataTypeBase()
    {
    }

    public long DataStartPosition { get; internal set; }

    public long DataEndPosition { get; internal set; }

    public virtual byte[] Data
    {
        get => _data;
        set
        {
            _data = value;
            SynchorizeData(true);
        }
    }

    public virtual int DataSize => Data.Length;

    public virtual Type MathedClrType => null;

    public virtual Encoding TextEncoding
    {
        get => _textEncoding ?? Encoding.UTF8;
        set => _textEncoding = value;
    }

    protected virtual void WriteTo(Stream stream)
    {
        stream.Write(Data, 0, Data.Length);
    }

    protected abstract void SynchorizeData(bool fromDataToValue);
}

public abstract class DataTypeBase<T> : DataTypeBase
{
    protected T _value;

    public DataTypeBase(T value)
    {
        Value = value;
    }

    public DataTypeBase()
    {
    }

    public virtual T Value
    {
        get => _value;
        set
        {
            _value = value;
            SynchorizeData(false);
        }
    }
}