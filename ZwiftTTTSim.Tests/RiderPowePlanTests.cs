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
                PowerByPosition = new[] { 350, 300, 280, 250 },
                Rider = new RiderData { FTP = 300, Weight = 70 }
            };

        // Act
        var actualPower = riderPowerPlan.GetPowerByPosition(position);

        // Assert
        Assert.Equal(expectedPower, actualPower);
    }
}
