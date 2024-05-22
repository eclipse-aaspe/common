using System.Reflection;
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
    [InlineData("[true, false, null, \"string\", 123, 123.45]", "[true,false,\"string\",123,123.45]")]
    [InlineData("[\"\\u0041\"]", "[\"A\"]")]
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

        // Access private _buffer field and set it to a new StringBuilder instance
        var bufferField = typeof(JsonCanonicalizer).GetField("_buffer", BindingFlags.NonPublic | BindingFlags.Instance);
        bufferField.SetValue(canonicalizer, new StringBuilder());

        // Access the private SerializeString method
        var serializeStringMethod = typeof(JsonCanonicalizer).GetMethod("SerializeString", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        serializeStringMethod.Invoke(canonicalizer, new object[] {input});
        var result = bufferField.GetValue(canonicalizer).ToString();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData('\n', "\\n")]
    [InlineData('\b', "\\b")]
    [InlineData('\f', "\\f")]
    [InlineData('\r', "\\r")]
    [InlineData('\t', "\\t")]
    [InlineData('"', "\\\"")]
    [InlineData('\\', "\\\\")]
    public void Escape_ShouldAppendEscapedCharacter(char input, string expected)
    {
        // Arrange
        var canonicalizer = new JsonCanonicalizer("{}");

        // Create a new StringBuilder instance to pass to the method
        var result = new StringBuilder();

        // Get the EscapeCharacter method using reflection
        var escapeMethod = typeof(JsonCanonicalizer).GetMethod("EscapeCharacter", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        escapeMethod.Invoke(canonicalizer, new object[] {result, input});

        // Assert
        result.ToString().Should().Be(expected);
    }

    [Fact]
    public void Serialize_WithNull_ShouldAppendNull()
    {
        // Arrange
        var canonicalizer = new JsonCanonicalizer("{}");

        // Access private _buffer field and set it to a new StringBuilder instance
        var bufferField = typeof(JsonCanonicalizer).GetField("_buffer", BindingFlags.NonPublic | BindingFlags.Instance);
        bufferField.SetValue(canonicalizer, new StringBuilder());

        // Access the private Serialize method
        var serializeMethod = typeof(JsonCanonicalizer).GetMethod("Serialize", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        serializeMethod.Invoke(canonicalizer, new object[] {null});
        var result = bufferField.GetValue(canonicalizer).ToString();

        // Assert
        result.Should().Be("null");
    }

    [Fact]
    public void Serialize_WithSortedDictionary_ShouldSerializeObject()
    {
        // Arrange
        var sortedDict = new SortedDictionary<string, object> {{"b", "2"}, {"a", "1"}};
        var canonicalizer = new JsonCanonicalizer("{}");

        // Access private _buffer field and set it to a new StringBuilder instance
        var bufferField = typeof(JsonCanonicalizer).GetField("_buffer", BindingFlags.NonPublic | BindingFlags.Instance);
        bufferField.SetValue(canonicalizer, new StringBuilder());

        // Access the private Serialize method
        var serializeMethod = typeof(JsonCanonicalizer).GetMethod("Serialize", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        serializeMethod.Invoke(canonicalizer, new object[] {sortedDict});
        var result = bufferField.GetValue(canonicalizer).ToString();

        // Assert
        result.Should().Be("{\"a\":\"1\",\"b\":\"2\"}");
    }

    [Fact]
    public void Serialize_WithList_ShouldSerializeArray()
    {
        // Arrange
        var list = new List<object?> {"1", "string", true};
        var canonicalizer = new JsonCanonicalizer("{}");

        // Access private _buffer field and set it to a new StringBuilder instance
        var bufferField = typeof(JsonCanonicalizer).GetField("_buffer", BindingFlags.NonPublic | BindingFlags.Instance);
        bufferField.SetValue(canonicalizer, new StringBuilder());

        // Access the private Serialize method
        var serializeMethod = typeof(JsonCanonicalizer).GetMethod("Serialize", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        serializeMethod.Invoke(canonicalizer, new object[] {list});
        var result = bufferField.GetValue(canonicalizer).ToString();

        // Assert
        result.Should().Be("[\"1\",\"string\",true]");
    }

    [Fact]
    public void Serialize_WithString_ShouldSerializeString()
    {
        // Arrange
        var input = "string";
        var canonicalizer = new JsonCanonicalizer("{}");

        // Access private _buffer field and set it to a new StringBuilder instance
        var bufferField = typeof(JsonCanonicalizer).GetField("_buffer", BindingFlags.NonPublic | BindingFlags.Instance);
        bufferField.SetValue(canonicalizer, new StringBuilder());

        // Access the private Serialize method
        var serializeMethod = typeof(JsonCanonicalizer).GetMethod("Serialize", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        serializeMethod.Invoke(canonicalizer, new object[] {input});
        var result = bufferField.GetValue(canonicalizer).ToString();

        // Assert
        result.Should().Be("\"string\"");
    }

    [Fact]
    public void Serialize_WithBool_ShouldSerializeBoolean()
    {
        // Arrange
        var input = true;
        var canonicalizer = new JsonCanonicalizer("{}");

        // Access private _buffer field and set it to a new StringBuilder instance
        var bufferField = typeof(JsonCanonicalizer).GetField("_buffer", BindingFlags.NonPublic | BindingFlags.Instance);
        bufferField.SetValue(canonicalizer, new StringBuilder());

        // Access the private Serialize method
        var serializeMethod = typeof(JsonCanonicalizer).GetMethod("Serialize", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        serializeMethod.Invoke(canonicalizer, new object[] {input});
        var result = bufferField.GetValue(canonicalizer).ToString();

        // Assert
        result.Should().Be("true");
    }

    [Fact]
    public void Serialize_WithDouble_ShouldSerializeNumber()
    {
        // Arrange
        var input = 123.45;
        var canonicalizer = new JsonCanonicalizer("{}");

        // Access private _buffer field and set it to a new StringBuilder instance
        var bufferField = typeof(JsonCanonicalizer).GetField("_buffer", BindingFlags.NonPublic | BindingFlags.Instance);
        bufferField.SetValue(canonicalizer, new StringBuilder());

        // Access the private Serialize method
        var serializeMethod = typeof(JsonCanonicalizer).GetMethod("Serialize", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        serializeMethod.Invoke(canonicalizer, new object[] {input});
        var result = bufferField.GetValue(canonicalizer).ToString();

        // Assert
        result.Should().Be("123.45");
    }

    [Fact]
    public void Serialize_WithUnknownType_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var input = new DateTime(2024, 1, 1);
        var canonicalizer = new JsonCanonicalizer("{}");

        // Access the private Serialize method
        var serializeMethod = typeof(JsonCanonicalizer).GetMethod("Serialize", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act & Assert
        Action act = () => serializeMethod.Invoke(canonicalizer, new object[] {input});
        act.Should().Throw<TargetInvocationException>().WithInnerException<InvalidOperationException>()
            .WithMessage($"Unknown object type: {input.GetType()}");
    }
}