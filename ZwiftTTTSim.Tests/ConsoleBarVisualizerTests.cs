using Xunit;
using ZwiftTTTSim.Core.Model;
using ZwiftTTTSim.Core.Exporters;

namespace ZwiftTTTSim.Tests;

public class ConsoleBarVisualizerTests
{
    [Fact]
    public void CreateBarVisualization_ShouldReturnEmptyString_WhenStepsListIsEmpty()
    {
        // Arrange
        var visualizer = new ConsoleBarVisualizer();
        var steps = new List<WorkoutStep>();

        // Act
        var result = visualizer.CreateBarVisualization("TestRider", steps, 4, 2);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void CreateBarVisualization_ShouldLimitToTwoRotations_WhenMoreRotationsProvided()
    {
        // Arrange
        var visualizer = new ConsoleBarVisualizer();
        var steps = new List<WorkoutStep>();
        
        // Create 12 steps (3 rotations x 4 riders)
        for (int i = 0; i < 12; i++)
        {
            steps.Add(new WorkoutStep
            {
                DurationSeconds = 60,
                Power = 250,
                Intensity = 0.85
            });
        }

        // Act
        var result = visualizer.CreateBarVisualization("TestRider", steps, 4, 3);

        // Assert
        // Should contain "First 2 Rotations" in the output
        Assert.Contains("First 2 Rotation", result);
        // Should only visualize 8 steps (2 rotations x 4 riders), not all 12
        // The exact length will depend on the bar width calculation
        Assert.NotEmpty(result);
    }

    [Fact]
    public void CreateBarVisualization_ShouldShowSingleRotation_WhenOnlyOneRotationExists()
    {
        // Arrange
        var visualizer = new ConsoleBarVisualizer();
        var steps = new List<WorkoutStep>();
        
        // Create 4 steps (1 rotation x 4 riders)
        for (int i = 0; i < 4; i++)
        {
            steps.Add(new WorkoutStep
            {
                DurationSeconds = 60,
                Power = 250,
                Intensity = 0.85
            });
        }

        // Act
        var result = visualizer.CreateBarVisualization("TestRider", steps, 4, 1);

        // Assert
        // Should contain "First 1 Rotation" (singular form)
        Assert.Contains("First 1 Rotation", result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void CreateBarVisualization_ShouldIncludeColorCodes_ForDifferentIntensityZones()
    {
        // Arrange
        var visualizer = new ConsoleBarVisualizer();
        var steps = new List<WorkoutStep>
        {
            // Anaerobic zone
            new WorkoutStep { DurationSeconds = 60, Power = 350, Intensity = 1.20 },
            // Threshold zone
            new WorkoutStep { DurationSeconds = 60, Power = 280, Intensity = 0.95 },
            // Tempo zone
            new WorkoutStep { DurationSeconds = 60, Power = 240, Intensity = 0.80 },
            // Recovery zone
            new WorkoutStep { DurationSeconds = 60, Power = 150, Intensity = 0.50 }
        };

        // Act
        var result = visualizer.CreateBarVisualization("TestRider", steps, 4, 1);

        // Assert
        // Should contain color codes for different zones
        Assert.Contains("{COLOR:Red}", result);       // Anaerobic
        Assert.Contains("{COLOR:Yellow}", result);    // Threshold
        Assert.Contains("{COLOR:Green}", result);     // Tempo
        Assert.Contains("{COLOR:DarkGray}", result);  // Recovery
        Assert.Contains("{COLOR:RESET}", result);     // Reset at the end
    }

    [Fact]
    public void CreateBarVisualization_ShouldHandleMixedDurations()
    {
        // Arrange
        var visualizer = new ConsoleBarVisualizer();
        var steps = new List<WorkoutStep>
        {
            new WorkoutStep { DurationSeconds = 90, Power = 350, Intensity = 1.15 },
            new WorkoutStep { DurationSeconds = 45, Power = 250, Intensity = 0.85 },
            new WorkoutStep { DurationSeconds = 60, Power = 280, Intensity = 0.95 },
            new WorkoutStep { DurationSeconds = 30, Power = 200, Intensity = 0.70 }
        };

        // Act
        var result = visualizer.CreateBarVisualization("TestRider", steps, 4, 1);

        // Assert
        // Should create visualization with different bar widths based on duration
        Assert.NotEmpty(result);
        Assert.Contains("Power Profile", result);
    }

    [Fact]
    public void CreateBarVisualization_ShouldThrowArgumentNullException_WhenRiderNameIsNull()
    {
        // Arrange
        var visualizer = new ConsoleBarVisualizer();
        var steps = new List<WorkoutStep>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            visualizer.CreateBarVisualization(null!, steps, 4, 2));
    }

    [Fact]
    public void CreateBarVisualization_ShouldThrowArgumentNullException_WhenStepsIsNull()
    {
        // Arrange
        var visualizer = new ConsoleBarVisualizer();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            visualizer.CreateBarVisualization("TestRider", null!, 4, 2));
    }

    [Fact]
    public void RenderToConsole_ShouldThrowArgumentNullException_WhenVisualizationIsNull()
    {
        // Arrange
        var visualizer = new ConsoleBarVisualizer();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            visualizer.RenderToConsole(null!));
    }

    [Fact]
    public void CreateBarVisualization_ShouldIncludeAllIntensityZones()
    {
        // Arrange
        var visualizer = new ConsoleBarVisualizer();
        var steps = new List<WorkoutStep>
        {
            // Test all intensity zones
            new WorkoutStep { DurationSeconds = 30, Power = 380, Intensity = 1.25 },  // Anaerobic
            new WorkoutStep { DurationSeconds = 30, Power = 330, Intensity = 1.10 },  // VO2 Max
            new WorkoutStep { DurationSeconds = 30, Power = 280, Intensity = 0.93 },  // Threshold
            new WorkoutStep { DurationSeconds = 30, Power = 240, Intensity = 0.80 },  // Tempo
            new WorkoutStep { DurationSeconds = 30, Power = 200, Intensity = 0.67 },  // Endurance
            new WorkoutStep { DurationSeconds = 30, Power = 150, Intensity = 0.50 }   // Recovery
        };

        // Act
        var result = visualizer.CreateBarVisualization("TestRider", steps, 6, 1);

        // Assert
        // Verify all intensity zone colors are present
        Assert.Contains("{COLOR:Red}", result);         // Anaerobic
        Assert.Contains("{COLOR:DarkYellow}", result);  // VO2 Max
        Assert.Contains("{COLOR:Yellow}", result);      // Threshold
        Assert.Contains("{COLOR:Green}", result);       // Tempo
        Assert.Contains("{COLOR:Blue}", result);        // Endurance
        Assert.Contains("{COLOR:DarkGray}", result);    // Recovery
    }

    [Fact]
    public void RenderLegend_ShouldNotThrowException()
    {
        // Arrange
        var visualizer = new ConsoleBarVisualizer();

        // Act & Assert - should not throw
        var exception = Record.Exception(() => visualizer.RenderLegend());
        Assert.Null(exception);
    }
}
