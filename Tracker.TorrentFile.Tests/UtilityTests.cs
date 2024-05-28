using System.Text;
using Shouldly;
using Tracker.TorrentFile.Bencode;

namespace Tracker.TorrentFile.Tests;

[TestFixture]
public class UtilityTests
{
    [Test]
    public void Create_ByteStringDataType_ReturnsCorrectTypeAndValue()
    {
        var encoding = Encoding.UTF8;
        var value = "test";

        var result = Utility.Create(encoding, value);

        result.ShouldBeOfType<ByteStringDataType>();
        result.TextEncoding.ShouldBe(Encoding.UTF8);
        result.Value.ShouldBe(value);
    }

    [Test]
    public void AddToDictionary_StringValue_AddsCorrectly()
    {
        var dict = new DictionaryDataType { TextEncoding = Encoding.UTF8 };
        var key = "testKey";
        var value = "testValue";

        Utility.AddToDictionary(dict, key, value);
        var entry = dict.First();
        var actualValue = Encoding.UTF8.GetString(((ByteStringDataType)entry.Value).Data);

        dict.Count().ShouldBe(1);
        entry.Key.ShouldBe(key);
        actualValue.ShouldBe(value);
    }

    [Test]
    public void AddToDictionary_IntValue_AddsCorrectly()
    {
        var dict = new DictionaryDataType { TextEncoding = Encoding.UTF8 };
        var key = "testKey";
        var value = 42;

        Utility.AddToDictionary(dict, key, value);
        var entry = dict.First();
        var actualValue = (IntegerDataType)entry.Value;

        
        dict.Count().ShouldBe(1);
        entry.Key.ShouldBe(key);
        actualValue.Value.ShouldBe(value);
    }

    [Test]
    public void AddToDictionary_LongValue_AddsCorrectly()
    {
        var dict = new DictionaryDataType { TextEncoding = Encoding.UTF8 };
        var key = "testKey";
        var value = 42L;

        Utility.AddToDictionary(dict, key, value);
        var entry = dict.First();
        var actualValue = (IntegerDataType)entry.Value;

        dict.Count().ShouldBe(1);
        entry.Key.ShouldBe(key);
        actualValue.Value.ShouldBe(value);
    }

    [Test]
    public void AddToDictionary_WithEncoding_AddsCorrectly()
    {
        var dict = new DictionaryDataType();
        var encoding = Encoding.UTF8;
        var key = "testKey";
        var value = "testValue";

        Utility.AddToDictionary(dict, encoding, key, value);
        var entry = dict.First();
        var actualValue = encoding.GetString(((ByteStringDataType)entry.Value).Data);

        dict.Count().ShouldBe(1);
        entry.Key.ShouldBe(key);
        actualValue.ShouldBe(value);
    }
}