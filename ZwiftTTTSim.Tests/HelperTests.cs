using Xunit;
using ZwiftTTTSim.Core.Helpers;

namespace ZwiftTTTSim.Tests;

public class HelperTests
{
    [Theory]
    [InlineData(0, 0, 5, 0)]
    [InlineData(1, 0, 5, 1)]
    [InlineData(2, 0, 5, 2)]
    [InlineData(3, 0, 5, 3)]
    [InlineData(4, 0, 5, 4)]
    [InlineData(0, 1, 5, 4)]
    [InlineData(1, 1, 5, 0)]
    [InlineData(1, 2, 5, 4)]
    [InlineData(0, 4, 5, 1)]
    public void Helpers_GetRotationPositionShouldReturnCorrectPosition(int riderIndex, int pullIndex, int totalRiders, int expected)
    {
        // Act
        var actual = Helpers.GetRotationPosition(riderIndex, pullIndex, totalRiders);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetRotationPosition_ShouldThrowArgumentOutOfRangeException_WhenTotalRidersIsZeroOrNegative()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Helpers.GetRotationPosition(0, 0, 0));
        Assert.Equal("totalRiders", exception.ParamName);
        
        var exceptionNegative = Assert.Throws<ArgumentOutOfRangeException>(() => Helpers.GetRotationPosition(0, 0, -1));
        Assert.Equal("totalRiders", exceptionNegative.ParamName);
    }

}