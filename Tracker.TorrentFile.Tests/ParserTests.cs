using System.Text;
using Shouldly;
using Tracker.TorrentFile.Bencode;

namespace Tracker.TorrentFile.Tests;

[TestFixture]
public class BencodeParserTests
{
    [Test]
    public void Parse_NullTextEncoding_ThrowsArgumentNullException()
    {
        var stream = new MemoryStream();

        Should.Throw<ArgumentNullException>(() => BencodeParser.Parse(null, stream));
    }

    [Test]
    public void Parse_NullStream_ThrowsArgumentNullException()
    {
        var textEncoding = Encoding.UTF8;
        Should.Throw<ArgumentNullException>(() => BencodeParser.Parse(textEncoding, null));
    }

    [Test]
    public void Parse_EmptyStream_ReturnsEmptyList()
    {
        var textEncoding = Encoding.UTF8;
        var stream = new MemoryStream();
        var result = BencodeParser.Parse(textEncoding, stream);
        result.ShouldBeEmpty();
    }

    [Test]
    public void Parse_ValidInteger_ReturnsCorrectDataType()
    {
        var textEncoding = Encoding.UTF8;
        var bencodedData = "i42e";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(bencodedData));

        var result = BencodeParser.Parse(textEncoding, stream);

        result.Count.ShouldBe(1);
        result[0].ShouldBeOfType<IntegerDataType>();
        var integerData = (IntegerDataType)result[0];
        integerData.Value.ShouldBe(42);
    }

    [Test]
    public void Parse_ValidString_ReturnsCorrectDataType()
    {
        var textEncoding = Encoding.UTF8;
        var bencodedData = "5:hello";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(bencodedData));

        var result = BencodeParser.Parse(textEncoding, stream);

        result.Count().ShouldBe(1);
        result[0].ShouldBeOfType<ByteStringDataType>();
        var stringData = (ByteStringDataType)result[0];
        textEncoding.GetString(stringData.Data).ShouldBe("hello");
    }

    [Test]
    public void Parse_ValidList_ReturnsCorrectDataType()
    {
        var textEncoding = Encoding.UTF8;
        var bencodedData = "li42ee";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(bencodedData));

        var result = BencodeParser.Parse(textEncoding, stream);

        result.Count.ShouldBe(1);
        result[0].ShouldBeOfType<ListDataType>();
        var listData = (ListDataType)result[0];
        listData.Value.Count.ShouldBe(1);
        listData.Value[0].ShouldBeOfType<IntegerDataType>();
        var integerData = (IntegerDataType)listData.Value[0];
        integerData.Value.ShouldBe(42);
    }

    [Test]
    public void Parse_InvalidInteger_ThrowsException()
    {
        var textEncoding = Encoding.UTF8;
        var bencodedData = "i42x";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(bencodedData));

        Should.Throw<UnexpectEndException>(() => BencodeParser.Parse(textEncoding, stream));
    }

    [Test]
    public void Parse_InvalidString_ThrowsException()
    {
        var textEncoding = Encoding.UTF8;
        var bencodedData = "5:hell";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(bencodedData));
        Should.Throw<UnexpectEndException>(() => BencodeParser.Parse(textEncoding, stream));
    }

    [Test]
    public void Parse_InvalidList_ThrowsException()
    {
        var textEncoding = Encoding.UTF8;
        var bencodedData = "li42x";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(bencodedData));
        Should.Throw<UnexpectEndException>(() => BencodeParser.Parse(textEncoding, stream));

    }

    [Test]
    public void Parse_InvalidDictionary_ThrowsException()
    {
        var textEncoding = Encoding.UTF8;
        var bencodedData = "d3:key5:val";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(bencodedData));
        Should.Throw<UnexpectEndException>(() => BencodeParser.Parse(textEncoding, stream));
    }
}