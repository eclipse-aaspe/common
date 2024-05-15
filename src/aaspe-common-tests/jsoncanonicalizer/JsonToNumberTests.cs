using System.Globalization;
using FluentAssertions;
using Org.Webpki.JsonCanonicalizer;

namespace aaspe_common_tests.jsoncanonicalizer;

public class JsonToNumberTests
{
    [Theory]
    [InlineData("123", 123)]
    [InlineData("123.45", 123.45)]
    [InlineData("-123.45", -123.45)]
    [InlineData("1.23e2", 123)]
    [InlineData("-1.23e-2", -0.0123)]
    public void Convert_ShouldParseValidNumberStrings(string input, double expected)
    {
        // Act
        var result = JsonToNumber.Convert(input);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("123abc")]
    [InlineData("abc123")]
    public void Convert_ShouldThrowFormatException_ForInvalidNumberStrings(string input)
    {
        // Act
        Action act = () => JsonToNumber.Convert(input);

        // Assert
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Convert_ShouldThrowArgumentNullException_ForNullInput()
    {
        // Act
        Action act = () => JsonToNumber.Convert(null);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Convert_ShouldHandleInvariantCulture()
    {
        // Arrange
        const string numberString = "123.45";
        const double expected = 123.45;

        // Act
        var result = JsonToNumber.Convert(numberString);

        // Assert
        result.Should().Be(expected);
        CultureInfo.CurrentCulture.Should().NotBe(CultureInfo.InvariantCulture,"ensuring current culture does not affect the result");
    }
}