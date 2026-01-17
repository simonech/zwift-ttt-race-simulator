using Xunit;
using ZwiftTTTSim.Core.Model;

namespace ZwiftTTTSim.Tests;

public class RiderPowerPlanTests
{
    [Theory]
    [InlineData(0, 350)]
    [InlineData(1, 300)]
    [InlineData(2, 280)]
    [InlineData(3, 250)]
    [InlineData(4, 250)] // Beyond defined positions, should return last defined
    [InlineData(5, 250)] // Beyond defined positions, should return last defined
    [InlineData(6, 250)] // Beyond defined positions, should return last defined
    public void RiderPowerPlan_ShouldCorrectlyGetPowerAlsoForPositionsAfterFourth(int position, int expectedPower)
    {
        // Arrange
        var riderPowerPlan = new RiderPowerPlan
            {
                Name = "Alice",
                PullDuration = TimeSpan.FromSeconds(30),
                PowerByPosition = [350, 300, 280, 250],
                Rider = new RiderData { FTP = 300, Weight = 70 }
            };

        // Act
        var actualPower = riderPowerPlan.GetPowerByPosition(position);

        // Assert
        Assert.Equal(expectedPower, actualPower);
    }

    [Fact]
    public void GetPowerByPosition_ShouldThrowArgumentOutOfRangeException_WhenPositionIsNegative()
    {
        // Arrange
        var riderPowerPlan = new RiderPowerPlan
        {
            Name = "Bob",
            PullDuration = TimeSpan.FromSeconds(30),
            PowerByPosition = [350, 300, 280, 250],
            Rider = new RiderData { FTP = 300, Weight = 70 }
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => riderPowerPlan.GetPowerByPosition(-1));
        Assert.Equal("position", exception.ParamName);
    }

    [Fact]
    public void GetPowerByPosition_ShouldThrowInvalidOperationException_WhenPowerByPositionIsEmpty()
    {
        // Arrange
        var riderPowerPlan = new RiderPowerPlan
        {
            Name = "Charlie",
            PullDuration = TimeSpan.FromSeconds(30),
            PowerByPosition = [],
            Rider = new RiderData { FTP = 300, Weight = 70 }
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => riderPowerPlan.GetPowerByPosition(0));
    }
}
