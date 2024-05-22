using aaspe_common.jsoncanonicalizer;
using FluentAssertions;

namespace aaspe_common_tests.jsoncanonicalizer;

public class JsonScannerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_NullOrEmptyJsonData_ThrowsArgumentNullException(string jsonData)
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new JsonScanner(jsonData));
    }

    [Fact]
    public void PeekNextNonWhiteSpaceCharacter_WithNonWhiteSpaceCharacter_ReturnsCharacter()
    {
        // Arrange
        var jsonData = "  abc";
        var scanner = new JsonScanner(jsonData);

        // Act
        var result = scanner.PeekNextNonWhiteSpaceCharacter();

        // Assert
        result.Should().Be('a');
    }

    [Fact]
    public void ScanForNextCharacter_WithExpectedCharacter_Succeeds()
    {
        // Arrange
        var jsonData = "abc";
        var scanner = new JsonScanner(jsonData);

        // Act
        Action act = () => scanner.ScanForNextCharacter('a');

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void GetNextCharacter_WithJsonData_ReturnsNextCharacter()
    {
        // Arrange
        var jsonData = "abc";
        var scanner = new JsonScanner(jsonData);

        // Act
        var result = scanner.GetNextCharacter();

        // Assert
        result.Should().Be('a');
    }

    [Fact]
    public void Scan_WithJsonData_ReturnsNextNonWhiteSpaceCharacter()
    {
        // Arrange
        var jsonData = "  abc";
        var scanner = new JsonScanner(jsonData);

        // Act
        var result = scanner.Scan();

        // Assert
        result.Should().Be('a');
    }

    [Fact]
    public void IsIndexWithinJsonLength_WithIndexWithinLength_ReturnsTrue()
    {
        // Arrange
        var jsonData = "abc";
        var scanner = new JsonScanner(jsonData);

        // Act
        var result = scanner.IsIndexWithinJsonLength();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsNextCharacterWhiteSpace_WithWhiteSpace_ReturnsTrue()
    {
        // Arrange
        var jsonData = "  abc";
        var scanner = new JsonScanner(jsonData);

        // Act
        var result = scanner.IsNextCharacterWhiteSpace();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void MoveBackToPreviousCharacter_WithNonZeroIndex_DecrementsIndexByOne()
    {
        // Arrange
        var jsonData = "abc";
        var scanner = new JsonScanner(jsonData);
        scanner.GetNextCharacter();

        // Act
        scanner.MoveBackToPreviousCharacter();

        // Assert
        scanner.IsIndexWithinJsonLength().Should().BeTrue();
        scanner.PeekNextNonWhiteSpaceCharacter().Should().Be('a');
    }

    [Fact]
    public void MoveBackToPreviousCharacter_WithZeroIndex_DoesNotChangeIndex()
    {
        // Arrange
        var jsonData = "abc";
        var scanner = new JsonScanner(jsonData);

        // Act
        scanner.MoveBackToPreviousCharacter();

        // Assert
        scanner.IsIndexWithinJsonLength().Should().BeTrue();
        scanner.PeekNextNonWhiteSpaceCharacter().Should().Be('a');
    }
}