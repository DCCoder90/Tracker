using System.ComponentModel;

namespace Tracker.Filesys.UriProtocol;

public abstract class MagnetProtocolXtBase : INotifyPropertyChanged
{
    private MagnetProtocolXtType _type;

    protected MagnetProtocolXtBase(MagnetProtocolXtType type)
    {
        Type = type;
    }

    public MagnetProtocolXtType Type
    {
        get => _type;
        set
        {
            if (value == _type)
                return;
            _type = value;
            OnPropertyChanged("Type");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        var handler = PropertyChanged;
        if (handler != null)
            handler(this, new PropertyChangedEventArgs(propertyName));
    }

    public virtual string GetParameter()
    {
        return "urn:" + Type.ToString().ToLower() + ":";
    }

    public override string ToString()
    {
        return GetParameter();
    }
}