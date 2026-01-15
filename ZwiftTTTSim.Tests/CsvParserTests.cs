using Xunit;
using ZwiftTTTSim.Core.Services;

namespace ZwiftTTTSim.Tests;

public class CsvParserTests
{
    [Fact]
    public void ParseCsv_ValidSingleRider_ShouldParseCorrectly()
    {
        // Arrange
        var csvContent = "Alice, 70, 300, 30, 350, 300, 280, 250";
        var parser = new CsvParser();

        // Act
        var result = parser.ParseCsv(csvContent);

        // Assert
        Assert.Single(result);
        Assert.Equal("Alice", result[0].Name);
        Assert.Equal(TimeSpan.FromSeconds(30), result[0].PullDuration);
        Assert.Equal(new[] { 350, 300, 280, 250 }, result[0].PowerByPosition);
    }

    [Fact]
    public void ParseCsv_ValidMultipleRiders_ShouldParseCorrectly()
    {
        // Arrange
        var csvContent = @"Alice, 70, 300, 30, 350, 300, 280, 250
Bob, 75, 280, 45, 330, 290, 270, 240
Charlie, 72, 320, 60, 370, 320, 300, 270";
        var parser = new CsvParser();

        // Act
        var result = parser.ParseCsv(csvContent);

        // Assert
        Assert.Equal(3, result.Count);
        
        Assert.Equal("Alice", result[0].Name);
        Assert.Equal(TimeSpan.FromSeconds(30), result[0].PullDuration);
        Assert.Equal(new[] { 350, 300, 280, 250 }, result[0].PowerByPosition);
        
        Assert.Equal("Bob", result[1].Name);
        Assert.Equal(TimeSpan.FromSeconds(45), result[1].PullDuration);
        Assert.Equal(new[] { 330, 290, 270, 240 }, result[1].PowerByPosition);
        
        Assert.Equal("Charlie", result[2].Name);
        Assert.Equal(TimeSpan.FromSeconds(60), result[2].PullDuration);
        Assert.Equal(new[] { 370, 320, 300, 270 }, result[2].PowerByPosition);
    }

    [Fact]
    public void ParseCsv_WithComments_ShouldSkipCommentLines()
    {
        // Arrange
        var csvContent = @"# This is a comment
Alice, 70, 300, 30, 350, 300, 280, 250
# Another comment
Bob, 75, 280, 45, 330, 290, 270, 240";
        var parser = new CsvParser();

        // Act
        var result = parser.ParseCsv(csvContent);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Alice", result[0].Name);
        Assert.Equal("Bob", result[1].Name);
    }

    [Fact]
    public void ParseCsv_WithEmptyLines_ShouldSkipEmptyLines()
    {
        // Arrange
        var csvContent = @"Alice, 70, 300, 30, 350, 300, 280, 250

Bob, 75, 280, 45, 330, 290, 270, 240";
        var parser = new CsvParser();

        // Act
        var result = parser.ParseCsv(csvContent);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Alice", result[0].Name);
        Assert.Equal("Bob", result[1].Name);
    }

    [Fact]
    public void ParseCsv_EmptyContent_ShouldThrowException()
    {
        // Arrange
        var csvContent = "";
        var parser = new CsvParser();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => parser.ParseCsv(csvContent));
        Assert.Contains("No valid rider data found", exception.Message);
    }

    [Fact]
    public void ParseCsv_OnlyComments_ShouldThrowException()
    {
        // Arrange
        var csvContent = @"# Comment 1
# Comment 2";
        var parser = new CsvParser();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => parser.ParseCsv(csvContent));
        Assert.Contains("No valid rider data found", exception.Message);
    }

    [Fact]
    public void ParseCsv_InsufficientFields_ShouldThrowException()
    {
        // Arrange
        var csvContent = "Alice, 70, 300";
        var parser = new CsvParser();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => parser.ParseCsv(csvContent));
        Assert.Contains("expected at least 7 fields", exception.Message);
    }

    [Fact]
    public void ParseCsv_InvalidWeight_ShouldThrowException()
    {
        // Arrange
        var csvContent = "Alice, invalid, 300, 30, 350, 300, 280, 250";
        var parser = new CsvParser();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => parser.ParseCsv(csvContent));
        Assert.Contains("Invalid weight value", exception.Message);
    }

    [Fact]
    public void ParseCsv_InvalidFTP_ShouldThrowException()
    {
        // Arrange
        var csvContent = "Alice, 70, invalid, 30, 350, 300, 280, 250";
        var parser = new CsvParser();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => parser.ParseCsv(csvContent));
        Assert.Contains("Invalid FTP value", exception.Message);
    }

    [Fact]
    public void ParseCsv_InvalidPullDuration_ShouldThrowException()
    {
        // Arrange
        var csvContent = "Alice, 70, 300, invalid, 350, 300, 280, 250";
        var parser = new CsvParser();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => parser.ParseCsv(csvContent));
        Assert.Contains("Invalid PullDuration value", exception.Message);
    }

    [Fact]
    public void ParseCsv_InvalidPowerValue_ShouldThrowException()
    {
        // Arrange
        var csvContent = "Alice, 70, 300, 30, invalid, 300, 280, 250";
        var parser = new CsvParser();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => parser.ParseCsv(csvContent));
        Assert.Contains("Invalid PowerByPosition[0] value", exception.Message);
    }

    [Fact]
    public void ParseCsv_MissingPowerValue_ShouldThrowException()
    {
        // Arrange
        var csvContent = "Alice, 70, 300, 30, 350, 300";
        var parser = new CsvParser();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => parser.ParseCsv(csvContent));
        Assert.Contains("expected at least 7 fields", exception.Message);
    }

    [Fact]
    public void ParseCsv_WithExtraSpaces_ShouldTrimAndParseCorrectly()
    {
        // Arrange
        var csvContent = "  Alice  ,  70  ,  300  ,  30  ,  350  ,  300  ,  280  ,  250  ";
        var parser = new CsvParser();

        // Act
        var result = parser.ParseCsv(csvContent);

        // Assert
        Assert.Single(result);
        Assert.Equal("Alice", result[0].Name);
        Assert.Equal(TimeSpan.FromSeconds(30), result[0].PullDuration);
        Assert.Equal(new[] { 350, 300, 280, 250 }, result[0].PowerByPosition);
    }
}
