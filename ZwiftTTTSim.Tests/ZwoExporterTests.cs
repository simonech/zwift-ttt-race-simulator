using Xunit;
using ZwiftTTTSim.Core.Model;
using ZwiftTTTSim.Core.Services;

namespace ZwiftTTTSim.Tests;

public class ZwoExporterTests
{
    [Fact]
    public void ZwoExporter_ShouldGenerateValidXml()
    {
        // Arrange
        var exporter = new ZwoExporter();
        var steps = new List<WorkoutStep>
        {
            new WorkoutStep { DurationSeconds = 30, Power = 350 },
            new WorkoutStep { DurationSeconds = 45, Power = 250 },
            new WorkoutStep { DurationSeconds = 60, Power = 270 }
        };

        // Act
        var result = exporter.ExportToZwo("TestRider", steps);

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains("<?xml version=", result);
        Assert.Contains("<workout_file>", result);
        Assert.Contains("<name>TTT simulation for TestRider</name>", result);
        Assert.Contains("<workout>", result);
        Assert.Contains("</workout>", result);
        Assert.Contains("</workout_file>", result);
    }

    [Fact]
    public void ZwoExporter_ShouldIncludeAllSteps()
    {
        // Arrange
        var exporter = new ZwoExporter();
        var steps = new List<WorkoutStep>
        {
            new WorkoutStep { DurationSeconds = 30, Power = 350 },
            new WorkoutStep { DurationSeconds = 45, Power = 250 },
            new WorkoutStep { DurationSeconds = 60, Power = 270 }
        };

        // Act
        var result = exporter.ExportToZwo("TestRider", steps);

        // Assert
        Assert.Contains("<SteadyState Duration=\"30\" Power=\"350\"", result);
        Assert.Contains("<SteadyState Duration=\"45\" Power=\"250\"", result);
        Assert.Contains("<SteadyState Duration=\"60\" Power=\"270\"", result);
    }

    [Fact]
    public void ZwoExporter_ShouldCreateFilesForAllRiders()
    {
        // Arrange
        var exporter = new ZwoExporter();
        var workouts = new Dictionary<string, List<WorkoutStep>>
        {
            ["Alice"] = new List<WorkoutStep>
            {
                new WorkoutStep { DurationSeconds = 30, Power = 350 }
            },
            ["Bob"] = new List<WorkoutStep>
            {
                new WorkoutStep { DurationSeconds = 45, Power = 330 }
            }
        };
        var outputDirectory = Path.Combine(Path.GetTempPath(), $"test_workouts_{Guid.NewGuid()}");

        try
        {
            // Act
            exporter.ExportToFiles(workouts, outputDirectory);

            // Assert
            Assert.True(Directory.Exists(outputDirectory));
            var files = Directory.GetFiles(outputDirectory, "*.zwo");
            Assert.Equal(2, files.Length);
            Assert.True(File.Exists(Path.Combine(outputDirectory, "Alice_TTT_Workout.zwo")));
            Assert.True(File.Exists(Path.Combine(outputDirectory, "Bob_TTT_Workout.zwo")));
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(outputDirectory))
            {
                Directory.Delete(outputDirectory, true);
            }
        }
    }

    [Theory]
    [InlineData("Alice", "Alice_TTT_Workout.zwo")]
    [InlineData("Bob Smith", "Bob Smith_TTT_Workout.zwo")]
    [InlineData("André", "André_TTT_Workout.zwo")]
    [InlineData("Øyvind", "Øyvind_TTT_Workout.zwo")]
    [InlineData("Petr Šimon", "Petr Šimon_TTT_Workout.zwo")]
    [InlineData("François", "François_TTT_Workout.zwo")]
    public void ZwoExporter_GetWorkoutFileName_ShouldSanitizeInvalidCharacters(string riderName, string expectedFileName)
    {
        // Act
        var result = ZwoExporter.GetWorkoutFileName(riderName);

        // Assert
        Assert.Equal(expectedFileName, result);
        // Verify no invalid filename characters remain
        var invalidChars = Path.GetInvalidFileNameChars();
        Assert.DoesNotContain(result, c => invalidChars.Contains(c));
    }

    [Fact]
    public void ZwoExporter_GetWorkoutFileName_ShouldSanitizeForwardSlash()
    {
        // Forward slash is invalid on all platforms
        var result = ZwoExporter.GetWorkoutFileName("Test/Rider");
        
        Assert.Equal("Test_Rider_TTT_Workout.zwo", result);
        var invalidChars = Path.GetInvalidFileNameChars();
        Assert.DoesNotContain(result, c => invalidChars.Contains(c));
    }

    [Fact]
    public void ZwoExporter_GetWorkoutFileName_ShouldThrowOnNullRiderName()
    {
        Assert.Throws<ArgumentNullException>(() => ZwoExporter.GetWorkoutFileName(null!));
    }

    [Fact]
    public void ZwoExporter_ExportToZwo_ShouldThrowOnNullRiderName()
    {
        var exporter = new ZwoExporter();
        var steps = new List<WorkoutStep> { new WorkoutStep { DurationSeconds = 30, Power = 350 } };
        
        Assert.Throws<ArgumentNullException>(() => exporter.ExportToZwo(null!, steps));
    }

    [Fact]
    public void ZwoExporter_ExportToZwo_ShouldThrowOnNullSteps()
    {
        var exporter = new ZwoExporter();
        
        Assert.Throws<ArgumentNullException>(() => exporter.ExportToZwo("TestRider", null!));
    }

    [Fact]
    public void ZwoExporter_ExportToFiles_ShouldThrowOnNullWorkouts()
    {
        var exporter = new ZwoExporter();
        
        Assert.Throws<ArgumentNullException>(() => exporter.ExportToFiles(null!, "output"));
    }

    [Fact]
    public void ZwoExporter_ExportToFiles_ShouldThrowOnNullOutputDirectory()
    {
        var exporter = new ZwoExporter();
        var workouts = new Dictionary<string, List<WorkoutStep>>();
        
        Assert.Throws<ArgumentNullException>(() => exporter.ExportToFiles(workouts, null!));
    }
}
