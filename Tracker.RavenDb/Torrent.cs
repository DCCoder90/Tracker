namespace Tracker.RavenDb;

internal class Torrent
{
    public string Hash { get; set; }
    public int Seeders { get; set; }
    public int Leechers { get; set; }
}