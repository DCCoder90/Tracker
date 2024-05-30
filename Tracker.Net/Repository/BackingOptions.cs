namespace Tracker.Data.Repository;

public class BackingOptions
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Database { get; set; }
    public bool UsesAuthentication { get; set; }
    public string CertificatePath { get; set; }
}