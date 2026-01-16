using Xunit;
using ZwiftTTTSim.Core.Model;

namespace ZwiftTTTSim.Tests;

public class WorkoutStepTests
{
    [Theory]
    [InlineData(280, 280, 1.0)]
    [InlineData(300, 280, 1.07)]
    [InlineData(250, 280, 0.89)]
    public void WorkoutStep_Intensity_ShouldCalculateCorrectly(double power, double ftp, double expectedIntensity)
    {
        // Arrange
        var step = new WorkoutStep { Power = power };
        step.SetIntensity(ftp);

        // Act
        var intensity = step.Intensity;

        // Assert
        Assert.Equal(expectedIntensity, intensity, 2);
    }

    [Fact]
    public void WorkoutStep_SetIntensity_ShouldThrowExceptionForZeroOrNegativeFTP()
    {
        // Arrange
        var step = new WorkoutStep { Power = 250 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => step.SetIntensity(0));
        Assert.Throws<ArgumentException>(() => step.SetIntensity(-100));
    }
}
