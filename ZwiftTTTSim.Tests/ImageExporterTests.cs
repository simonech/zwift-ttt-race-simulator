using Xunit;
using ZwiftTTTSim.Core.Model;
using ZwiftTTTSim.Core.Exporters;

namespace ZwiftTTTSim.Tests;

public class ImageExporterTests
{
    [Fact]
    public void ImageExporter_ShouldGenerateValidPngData()
    {
        // Arrange
        var exporter = new ImageExporter();
        var steps = new List<WorkoutStep>
        {
            new WorkoutStep { DurationSeconds = 30, Power = 350 },
            new WorkoutStep { DurationSeconds = 45, Power = 250 },
            new WorkoutStep { DurationSeconds = 60, Power = 270 }
        };

        // Act
        var result = exporter.ExportToImage("TestRider", steps, 0, 3);

        // Assert
        Assert.NotEmpty(result);
        // PNG files start with the magic bytes 0x89, 0x50, 0x4E, 0x47
        Assert.Equal(0x89, result[0]);
        Assert.Equal(0x50, result[1]);
        Assert.Equal(0x4E, result[2]);
        Assert.Equal(0x47, result[3]);
    }

    [Fact]
    public void ImageExporter_ShouldCreateFilesForAllRiders()
    {
        // Arrange
        var exporter = new ImageExporter();
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
        var outputDirectory = Path.Combine(Path.GetTempPath(), $"test_images_{Guid.NewGuid()}");

        try
        {
            // Act
            exporter.ExportToFiles(workouts, outputDirectory, 2);

            // Assert
            Assert.True(Directory.Exists(outputDirectory));
            var files = Directory.GetFiles(outputDirectory, "*.png");
            Assert.Equal(2, files.Length);
            Assert.True(File.Exists(Path.Combine(outputDirectory, "Alice_TTT_Workout.png")));
            Assert.True(File.Exists(Path.Combine(outputDirectory, "Bob_TTT_Workout.png")));
            
            // Verify files are not empty
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                Assert.True(fileInfo.Length > 0);
            }
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
    [InlineData("Alice", "Alice_TTT_Workout.png")]
    [InlineData("Bob Smith", "Bob Smith_TTT_Workout.png")]
    [InlineData("André", "André_TTT_Workout.png")]
    [InlineData("Øyvind", "Øyvind_TTT_Workout.png")]
    [InlineData("Petr Šimon", "Petr Šimon_TTT_Workout.png")]
    [InlineData("François", "François_TTT_Workout.png")]
    public void ImageExporter_GetWorkoutImageFileName_ShouldSanitizeInvalidCharacters(string riderName, string expectedFileName)
    {
        // Act
        var result = ImageExporter.GetWorkoutImageFileName(riderName);

        // Assert
        Assert.Equal(expectedFileName, result);
        // Verify no invalid filename characters remain
        var invalidChars = Path.GetInvalidFileNameChars();
        Assert.DoesNotContain(result, c => invalidChars.Contains(c));
    }

    [Fact]
    public void ImageExporter_GetWorkoutImageFileName_ShouldSanitizeForwardSlash()
    {
        // Forward slash is invalid on all platforms
        var result = ImageExporter.GetWorkoutImageFileName("Test/Rider");
        
        Assert.Equal("Test_Rider_TTT_Workout.png", result);
        var invalidChars = Path.GetInvalidFileNameChars();
        Assert.DoesNotContain(result, c => invalidChars.Contains(c));
    }

    [Fact]
    public void ImageExporter_GetWorkoutImageFileName_ShouldThrowOnNullRiderName()
    {
        Assert.Throws<ArgumentNullException>(() => ImageExporter.GetWorkoutImageFileName(null!));
    }

    [Fact]
    public void ImageExporter_ExportToImage_ShouldThrowOnNullRiderName()
    {
        var exporter = new ImageExporter();
        var steps = new List<WorkoutStep> { new WorkoutStep { DurationSeconds = 30, Power = 350 } };
        
        Assert.Throws<ArgumentNullException>(() => exporter.ExportToImage(null!, steps, 0, 1));
    }

    [Fact]
    public void ImageExporter_ExportToImage_ShouldThrowOnNullSteps()
    {
        var exporter = new ImageExporter();
        
        Assert.Throws<ArgumentNullException>(() => exporter.ExportToImage("TestRider", null!, 0, 1));
    }

    [Fact]
    public void ImageExporter_ExportToImage_ShouldThrowOnEmptySteps()
    {
        var exporter = new ImageExporter();
        var steps = new List<WorkoutStep>();
        
        Assert.Throws<ArgumentException>(() => exporter.ExportToImage("TestRider", steps, 0, 1));
    }

    [Fact]
    public void ImageExporter_ExportToFiles_ShouldThrowOnNullWorkouts()
    {
        var exporter = new ImageExporter();
        
        Assert.Throws<ArgumentNullException>(() => exporter.ExportToFiles(null!, "output", 1));
    }

    [Fact]
    public void ImageExporter_ExportToFiles_ShouldThrowOnNullOutputDirectory()
    {
        var exporter = new ImageExporter();
        var workouts = new Dictionary<string, List<WorkoutStep>>();
        
        Assert.Throws<ArgumentNullException>(() => exporter.ExportToFiles(workouts, null!, 1));
    }

    [Fact]
    public void ImageExporter_ShouldHandleMultipleStepsWithDifferentDurations()
    {
        // Arrange
        var exporter = new ImageExporter();
        var steps = new List<WorkoutStep>
        {
            new WorkoutStep { DurationSeconds = 30, Power = 350 },
            new WorkoutStep { DurationSeconds = 90, Power = 280 },
            new WorkoutStep { DurationSeconds = 45, Power = 320 },
            new WorkoutStep { DurationSeconds = 60, Power = 270 }
        };

        // Act
        var result = exporter.ExportToImage("TestRider", steps, 0, 4);

        // Assert
        Assert.NotEmpty(result);
        Assert.True(result.Length > 1000); // Reasonable size for a PNG image
    }

    [Fact]
    public void ImageExporter_ShouldHandleVariousPowerLevels()
    {
        // Arrange
        var exporter = new ImageExporter();
        var steps = new List<WorkoutStep>
        {
            new WorkoutStep { DurationSeconds = 30, Power = 400 }, // High power
            new WorkoutStep { DurationSeconds = 30, Power = 200 }, // Low power
            new WorkoutStep { DurationSeconds = 30, Power = 300 }  // Medium power
        };

        // Act
        var result = exporter.ExportToImage("TestRider", steps, 0, 3);

        // Assert
        Assert.NotEmpty(result);
        Assert.True(result.Length > 1000);
    }

    [Fact]
    public void ImageExporter_ShouldHandleRecoveryZonePower()
    {
        // Arrange
        var exporter = new ImageExporter();
        var steps = new List<WorkoutStep>
        {
            new WorkoutStep { DurationSeconds = 60, Power = 150 }, // Recovery zone power (assuming FTP ~300, this is ~50% FTP)
            new WorkoutStep { DurationSeconds = 60, Power = 180 }, // Still recovery zone (~60% FTP)
            new WorkoutStep { DurationSeconds = 60, Power = 300 }  // Higher power
        };

        // Act
        var result = exporter.ExportToImage("TestRider", steps, 0, 3);

        // Assert
        Assert.NotEmpty(result);
        Assert.True(result.Length > 1000);
        // Verify PNG magic bytes
        Assert.Equal(0x89, result[0]);
        Assert.Equal(0x50, result[1]);
        Assert.Equal(0x4E, result[2]);
        Assert.Equal(0x47, result[3]);
    }
}
