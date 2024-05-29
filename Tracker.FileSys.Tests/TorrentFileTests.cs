using System.Text;
using Shouldly;
using Tracker.Filesys.Torrent;

namespace Tracker.FileSys.Tests;

[TestFixture]
public class TorrentFileTests
{
    [Test]
    public void TorrentFile_DefaultConstructor_InitializesCorrectly()
    {
        var torrentFile = new TorrentFile();

        torrentFile.AnnounceList.ShouldNotBeNull();
        torrentFile.MetaInfo.ShouldNotBeNull();
        torrentFile.Encoding.ShouldBe(Encoding.UTF8);
        torrentFile.CreationDate.ShouldBeNull();
        torrentFile.Comment.ShouldBeNull();
        torrentFile.CreatedBy.ShouldBeNull();
        torrentFile.Nodes.ShouldBeNull();
        torrentFile.HttpSeeds.ShouldBeNull();
        torrentFile.IsPrivate.ShouldBeFalse();
        torrentFile.MetaInfoHash.ShouldBeNull();
        torrentFile.MetaInfoHashString.ShouldBeNull();
    }

    [Test]
    public void TorrentFile_PathConstructor_LoadsCorrectly()
    {
        var path = "Test.torrent";

        var torrentFile = new TorrentFile(path);
        var fileNames = torrentFile.MetaInfo.Files.Select(x => x.Name).ToList();

        torrentFile.Path.ShouldBe(path);
        torrentFile.Announce.ShouldBe("udp://127.0.0.1:1234");
        torrentFile.Comment.ShouldBe("Test");
        fileNames.ShouldContain("TestFile.txt");
        fileNames.ShouldContain("TestFile2.txt");
    }

    [Test]
    public void Name_Property_GetSetCorrectly()
    {
        var torrentFile = new TorrentFile();
        var name = "TestName";

        torrentFile.Name = name;

        torrentFile.Name.ShouldBe(name);
        torrentFile.MetaInfo.Name.ShouldBe(name);
    }
}