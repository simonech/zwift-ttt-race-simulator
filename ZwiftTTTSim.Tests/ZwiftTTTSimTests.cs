using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Xunit;
using ZwiftTTTSim.Core.Model;
using ZwiftTTTSim.Core.Services;

namespace ZwiftTTTSim.Tests;

public class ZwiftTTTSimTests
{
    // MemberData providing power plan sets for team sizes 4, 6, and 8 with rotations 1..3
    public static IEnumerable<object[]> PowerPlanSets()
    {
        yield return new object[] { TestData.GetSampleRiderPowerPlans(), 1 };
        yield return new object[] { TestData.GetSampleRiderPowerPlans(), 2 };
        yield return new object[] { TestData.GetSampleRiderPowerPlans(), 3 };

        yield return new object[] { TestData.GetSampleRiderPowerPlans6(), 1 };
        yield return new object[] { TestData.GetSampleRiderPowerPlans6(), 2 };
        yield return new object[] { TestData.GetSampleRiderPowerPlans6(), 3 };

        yield return new object[] { TestData.GetSampleRiderPowerPlans8(), 1 };
        yield return new object[] { TestData.GetSampleRiderPowerPlans8(), 2 };
        yield return new object[] { TestData.GetSampleRiderPowerPlans8(), 3 };
    }

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

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void WorkoutCreator_ShouldAlwaysCreateSameNumberOfWorkoutsAsPowerPlans(int rotations)
    {
        // Arrange
        var powerPlans = TestData.GetSampleRiderPowerPlans();
        var workoutCreator = new WorkoutCreator();

        // Act
        var workouts = workoutCreator.CreateWorkouts(powerPlans, rotations);

        // Assert
        Assert.Equal(powerPlans.Count, workouts.Count);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void WorkoutCreator_ShouldCreateStepsEqualToRidersTimesRotations(int rotations)
    {
        // Arrange
        var powerPlans = TestData.GetSampleRiderPowerPlans();
        var workoutCreator = new WorkoutCreator();
        var expectedStepsCount = powerPlans.Count * rotations;

        // Act
        var workouts = workoutCreator.CreateWorkouts(powerPlans, rotations);

        // Assert
        foreach (var workout in workouts.Values)
        {
            Assert.Equal(expectedStepsCount, workout.Count);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void WorkoutCreator_ShouldHaveCorrectPowerValuesForEachRider(int rotations)
    {
        // Arrange
        var powerPlans = TestData.GetSampleRiderPowerPlans();
        var workoutCreator = new WorkoutCreator();

        // Act
        var workouts = workoutCreator.CreateWorkouts(powerPlans, rotations);

        // Assert - Verify each rider's power values are correct
        for (int riderIndex = 0; riderIndex < powerPlans.Count; riderIndex++)
        {
            var riderName = powerPlans[riderIndex].Name;
            var actualSteps = workouts[riderName];

            for (int stepIndex = 0; stepIndex < actualSteps.Count; stepIndex++)
            {
                var pullingRiderIndex = stepIndex % powerPlans.Count;
                // Compute rider's position at this step relative to the pulling rider
                var position = (riderIndex - pullingRiderIndex + powerPlans.Count) % powerPlans.Count;
                var clampedIndex = Math.Min(position, powerPlans[riderIndex].PowerByPosition.Length - 1);
                var expectedPower = powerPlans[riderIndex].PowerByPosition[clampedIndex];

                Assert.Equal(expectedPower, actualSteps[stepIndex].Power);
            }
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void WorkoutCreator_ShouldHaveCorrectDurationValuesForEachStep(int rotations)
    {
        // Arrange
        var powerPlans = TestData.GetSampleRiderPowerPlans();
        var workoutCreator = new WorkoutCreator();


        // Act
        var workouts = workoutCreator.CreateWorkouts(powerPlans, rotations);

        // Assert - Verify each step has the correct duration based on the pulling rider
        for (int riderIndex = 0; riderIndex < powerPlans.Count; riderIndex++)
        {
            var riderName = powerPlans[riderIndex].Name;
            var actualSteps = workouts[riderName];

            for (int stepIndex = 0; stepIndex < actualSteps.Count; stepIndex++)
            {
                var pullingRiderIndex = stepIndex % powerPlans.Count;
                var expectedDurationSeconds = (int)powerPlans[pullingRiderIndex].PullDuration.TotalSeconds;

                Assert.Equal(expectedDurationSeconds, actualSteps[stepIndex].DurationSeconds);
            }
        }
    }

    [Theory]
    [MemberData(nameof(PowerPlanSets))]
    public void WorkoutCreator_ShouldSupportTeamsOfVariousSizes(List<RiderPowerPlan> powerPlans, int rotations)
    {
        var workoutCreator = new WorkoutCreator();
        var expectedStepsCount = powerPlans.Count * rotations;

        var workouts = workoutCreator.CreateWorkouts(powerPlans, rotations);

        // Count checks
        Assert.Equal(powerPlans.Count, workouts.Count);
        foreach (var workout in workouts.Values)
        {
            Assert.Equal(expectedStepsCount, workout.Count);
        }

        // Duration and power checks
        for (int riderIndex = 0; riderIndex < powerPlans.Count; riderIndex++)
        {
            var riderName = powerPlans[riderIndex].Name;
            var actualSteps = workouts[riderName];

            for (int stepIndex = 0; stepIndex < actualSteps.Count; stepIndex++)
            {
                var pullingRiderIndex = stepIndex % powerPlans.Count;
                var expectedDurationSeconds = (int)powerPlans[pullingRiderIndex].PullDuration.TotalSeconds;
                Assert.Equal(expectedDurationSeconds, actualSteps[stepIndex].DurationSeconds);

                var position = (riderIndex - pullingRiderIndex + powerPlans.Count) % powerPlans.Count;
                var clampedIndex = Math.Min(position, powerPlans[riderIndex].PowerByPosition.Length - 1);
                var expectedPower = powerPlans[riderIndex].PowerByPosition[clampedIndex];
                Assert.Equal(expectedPower, actualSteps[stepIndex].Power);
            }
        }
    }

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
}
