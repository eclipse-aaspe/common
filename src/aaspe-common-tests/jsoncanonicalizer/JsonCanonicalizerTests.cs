using System.Text;
using FluentAssertions;
using Org.Webpki.JsonCanonicalizer;

namespace aaspe_common_tests.jsoncanonicalizer;

public class JsonCanonicalizerTests
{
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

    [Theory]
    [InlineData("simple", "\"simple\"")]
    [InlineData("with \"quotes\"", "\"with \\\"quotes\\\"\"")]
    [InlineData("backslash\\", "\"backslash\\\\\"")]
    [InlineData("newline\n", "\"newline\\n\"")]
    [InlineData("backspace\b", "\"backspace\\b\"")]
    [InlineData("formfeed\f", "\"formfeed\\f\"")]
    [InlineData("carriage\r", "\"carriage\\r\"")]
    [InlineData("tab\t", "\"tab\\t\"")]
    [InlineData("\u0019", "\"\\u0019\"")] // Control character
    public void SerializeString_ShouldHandleAllCases(string input, string expected)
    {
        // Arrange
        var canonicalizer = new JsonCanonicalizer("{}");

        // Act
        var bufferField = typeof(JsonCanonicalizer).GetField("buffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        bufferField.SetValue(canonicalizer, new StringBuilder());
        var serializeStringMethod = typeof(JsonCanonicalizer).GetMethod("SerializeString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        serializeStringMethod.Invoke(canonicalizer, new object[] {input});
        var result = bufferField.GetValue(canonicalizer).ToString();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData('n', "\\n")]
    [InlineData('b', "\\b")]
    [InlineData('f', "\\f")]
    [InlineData('r', "\\r")]
    [InlineData('t', "\\t")]
    [InlineData('"', "\\\"")]
    [InlineData('\\', "\\\\")]
    public void Escape_ShouldAppendEscapedCharacter(char input, string expected)
    {
        // Arrange
        var canonicalizer = new JsonCanonicalizer("{}");

        // Act
        var bufferField = typeof(JsonCanonicalizer).GetField("buffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        bufferField.SetValue(canonicalizer, new StringBuilder());
        var escapeMethod = typeof(JsonCanonicalizer).GetMethod("Escape", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        escapeMethod.Invoke(canonicalizer, new object[] {input});
        var result = bufferField.GetValue(canonicalizer).ToString();

        // Assert
        result.Should().Be(expected);
    }
}