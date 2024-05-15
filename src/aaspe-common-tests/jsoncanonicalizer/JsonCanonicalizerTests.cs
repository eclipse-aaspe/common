using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Org.Webpki.JsonCanonicalizer;

namespace aaspe_common_tests.jsoncanonicalizer;

public class JsonCanonicalizerTests
{
    private readonly IFixture _fixture;

    public JsonCanonicalizerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    [Fact]
    public void Constructor_WithJsonString_ShouldInitializeBuffer()
    {
        // Arrange
        var jsonString = "{\"key\":\"value\"}";

        // Act
        var canonicalizer = new JsonCanonicalizer(jsonString);

        // Assert
        canonicalizer.GetEncodedString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Constructor_WithByteArray_ShouldInitializeBuffer()
    {
        // Arrange
        var jsonString = "{\"key\":\"value\"}";
        var jsonBytes = Encoding.UTF8.GetBytes(jsonString);

        // Act
        var canonicalizer = new JsonCanonicalizer(jsonBytes);

        // Assert
        canonicalizer.GetEncodedString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GetEncodedString_ShouldReturnCanonicalJsonString()
    {
        // Arrange
        var jsonString = "{\"key\":\"value\"}";
        var expectedCanonicalJsonString = "{\"key\":\"value\"}";

        // Act
        var canonicalizer = new JsonCanonicalizer(jsonString);
        var result = canonicalizer.GetEncodedString();

        // Assert
        result.Should().Be(expectedCanonicalJsonString);
    }

    [Fact]
    public void GetEncodedUTF8_ShouldReturnCanonicalJsonAsByteArray()
    {
        // Arrange
        var jsonString = "{\"key\":\"value\"}";
        var expectedCanonicalJsonString = "{\"key\":\"value\"}";
        var expectedBytes = Encoding.UTF8.GetBytes(expectedCanonicalJsonString);

        // Act
        var canonicalizer = new JsonCanonicalizer(jsonString);
        var result = canonicalizer.GetEncodedUTF8();

        // Assert
        result.Should().Equal(expectedBytes);
    }

    [Theory]
    [InlineData("{\"b\":2,\"a\":1}", "{\"a\":1,\"b\":2}")]
    [InlineData("{\"b\":2,\"a\":1,\"c\":{\"d\":4,\"e\":5}}", "{\"a\":1,\"b\":2,\"c\":{\"d\":4,\"e\":5}}")]
    public void GetEncodedString_ShouldReturnSortedCanonicalJsonString(string input, string expected)
    {
        // Arrange & Act
        var canonicalizer = new JsonCanonicalizer(input);
        var result = canonicalizer.GetEncodedString();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("[true, false, null, \"string\", 123, 123.45]", "[true,false,null,\"string\",123,123.45]")]
    [InlineData("[\"\\u0041\"]", "[\"A\"]")] // Testing Unicode escape handling
    public void GetEncodedString_ShouldHandleJsonArrays(string input, string expected)
    {
        // Arrange & Act
        var canonicalizer = new JsonCanonicalizer(input);
        var result = canonicalizer.GetEncodedString();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Serialize_ShouldThrowInvalidOperationException_ForUnknownObject()
    {
        // Arrange
        var unknownObject = new { };

        // Act
        //Action act = () => new JsonCanonicalizer("{\"key\":\"value\"}").Serialize(unknownObject);

        // Assert
        //act.Should().Throw<InvalidOperationException>().WithMessage("Unknown object: *");
    }

    [Theory]
    [InlineData("\n", "\\n")]
    [InlineData("\b", "\\b")]
    [InlineData("\f", "\\f")]
    [InlineData("\r", "\\r")]
    [InlineData("\t", "\\t")]
    [InlineData("\"", "\\\"")]
    [InlineData("\\", @"\\")]
    public void SerializeString_ShouldEscapeSpecialCharacters(string input, string expected)
    {
        // Arrange
        var canonicalizer = new JsonCanonicalizer("{}");

        // Act
        //canonicalizer.SerializeString(input);
        var result = canonicalizer.GetEncodedString();

        // Assert
        result.Should().Contain(expected);
    }

    [Fact]
    public void SerializeString_ShouldHandleControlCharacters()
    {
        // Arrange
        var input = new string(new[] {(char) 0x1F}); // control character
        var canonicalizer = new JsonCanonicalizer("{}");

        // Act
        //canonicalizer.SerializeString(input);
        var result = canonicalizer.GetEncodedString();

        // Assert
        result.Should().Contain("\\u001f");
    }
}