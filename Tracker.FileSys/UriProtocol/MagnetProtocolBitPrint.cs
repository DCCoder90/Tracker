using System.ComponentModel;

namespace Tracker.Filesys.UriProtocol;

public class MagnetProtocolBitPrint : MagnetProtocolXtHashedBase, INotifyPropertyChanged
{
    private byte[] _tthHash;
    private string _tthHashString;

    public MagnetProtocolBitPrint(byte[] hash, byte[] tthHash)
        : base(MagnetProtocolXtType.BitPrint, hash)
    {
        TthHash = tthHash;
    }

    public MagnetProtocolBitPrint(string hashString, string tthHashString)
        : base(MagnetProtocolXtType.BitPrint, hashString)
    {
        TthHashString = tthHashString;
    }

    public byte[] TthHash
    {
        get => _tthHash;
        set
        {
            if (Equals(value, _tthHash)) return;
            _tthHash = value;
            OnPropertyChanged("TthHash");
            _tthHashString = BitConverter.ToString(value).Replace("-", "");
        }
    }

    public string TthHashString
    {
        get => _tthHashString;
        set
        {
            if (value == _tthHashString) return;
            _tthHashString = value;
            OnPropertyChanged("TthHashString");
            _tthHash = Enumerable.Range(0, value.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(value.Substring(x, 2), 16))
                .ToArray();
        }
    }

    public override string GetParameter()
    {
        return base.GetParameter() + HashString + "." + TthHashString;
    }
}