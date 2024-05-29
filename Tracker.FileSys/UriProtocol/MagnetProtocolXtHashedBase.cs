using System.ComponentModel;

namespace Tracker.Filesys.UriProtocol;

public class MagnetProtocolXtHashedBase : MagnetProtocolXtBase, INotifyPropertyChanged
{
    private byte[] _hash;
    private string _hashString;

    public MagnetProtocolXtHashedBase(MagnetProtocolXtType type, byte[] hash)
        : base(type)
    {
        Hash = hash;
    }

    public MagnetProtocolXtHashedBase(MagnetProtocolXtType type, string hashString)
        : base(type)
    {
        HashString = hashString;
    }

    public byte[] Hash
    {
        get => _hash;
        set
        {
            if (Equals(value, _hash))
                return;
            _hash = value;
            OnPropertyChanged("Hash");
            _hashString = BitConverter.ToString(value).Replace("-", "");
        }
    }

    public string HashString
    {
        get => _hashString;
        set
        {
            if (value == _hashString) return;
            _hashString = value;
            OnPropertyChanged("HashString");

            _hash = Enumerable.Range(0, value.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(value.Substring(x, 2), 16))
                .ToArray();
        }
    }

    public override string GetParameter()
    {
        return base.GetParameter() + HashString;
    }

    public override string ToString()
    {
        return GetParameter();
    }
}