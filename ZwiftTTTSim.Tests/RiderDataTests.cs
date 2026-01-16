using Xunit;
using ZwiftTTTSim.Core.Model;

namespace ZwiftTTTSim.Tests;

public class RiderDataTests
{
    [Fact]
    public void RiderData_ShouldHaveCorrectWKg()
    {
        // Arrange
        var riderData = new RiderData { FTP = 280, Weight = 85 }; // Example values
        var expectedWKg = 3.29; // Calculate expected WKg

        // Act
        var actualWKg = riderData.WKg;

        // Assert
        Assert.Equal(expectedWKg, actualWKg, 2);
    }
}
