namespace Tracker.Filesys.Torrent;

[Flags]
public enum LoadFlag
{
    None = 0,
    LoadInfoSectionData = 1,
    ComputeMetaInfoHash = 2
}