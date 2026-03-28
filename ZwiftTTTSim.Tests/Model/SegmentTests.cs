using System;
using Xunit;
using ZwiftTTTSim.Core.Model.Segments;

namespace ZwiftTTTSim.Tests.Model;

public class SegmentTests
{
    [Fact]
    public void FlatSegment_CalculatesDuration_FromDistanceAndSpeed()
    {
        // Arrange
        var flatSegment = new FlatSegment
        {
            DistanceKm = 10,
            AvgSpeedKph = 40
        };

        // Act
        var result = flatSegment.Duration;

        // Assert
        Assert.Equal(TimeSpan.FromSeconds(900), result);
    }

    [Fact]
    public void FlatSegment_ReturnsZeroDuration_WhenSpeedIsZero()
    {
        // Arrange
        var flatSegment = new FlatSegment
        {
            DistanceKm = 10,
            AvgSpeedKph = 0
        };

        // Act
        var result = flatSegment.Duration;

        // Assert
        Assert.Equal(TimeSpan.Zero, result);
    }

    [Fact]
    public void ClimbSegment_ReturnsDuration_FromSeconds()
    {
        // Arrange
        var climbSegment = new ClimbSegment
        {
            DurationSeconds = 300
        };

        // Act
        var result = climbSegment.Duration;

        // Assert
        Assert.Equal(TimeSpan.FromMinutes(5), result);
    }
}
