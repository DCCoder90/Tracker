namespace Tracker.TorrentFile.UriProtocol;

internal class MagnetProtocolXtTth : MagnetProtocolXtHashedBase
{
	public MagnetProtocolXtTth(byte[] hash) : base(MagnetProtocolXtType.Tth, hash)
    {
    }

	public MagnetProtocolXtTth(string hashString) : base(MagnetProtocolXtType.Tth, hashString)
    {
    }

	public override string GetParameter()
    {
        return "urn:tree:tiger:" + HashString;
    }
}