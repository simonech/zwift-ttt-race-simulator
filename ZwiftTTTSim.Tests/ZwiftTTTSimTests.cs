using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Xunit;
using ZwiftTTTSim.Core.Model;
using ZwiftTTTSim.Core.Services;

namespace ZwiftTTTSim.Tests;

public class ZwiftTTTSimTests
{
    [Fact]
    public void RiderData_ShouldHaveCorrectWKg()
    {
        // Arrange
        var riderData = new RiderData { FTP = 280, Weight = 85 }; // Example values
        var expectedWKg = riderData.FTP / riderData.Weight; // Calculate expected WKg

        // Act
        var actualWKg = riderData.WKg;

        // Assert
        Assert.Equal(expectedWKg, actualWKg);
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
            int[] expectedPower =
            {
                powerPlans[riderIndex].PowerByPosition[(0+riderIndex)%powerPlans[riderIndex].PowerByPosition.Length],
                powerPlans[riderIndex].PowerByPosition[(3+riderIndex)%powerPlans[riderIndex].PowerByPosition.Length],
                powerPlans[riderIndex].PowerByPosition[(2+riderIndex)%powerPlans[riderIndex].PowerByPosition.Length],
                powerPlans[riderIndex].PowerByPosition[(1+riderIndex)%powerPlans[riderIndex].PowerByPosition.Length]
            };
            for (int stepIndex = 0; stepIndex < actualSteps.Count; stepIndex++)
            {
                var pullingRiderIndex = stepIndex % powerPlans.Count;
                //Console.WriteLine($"Step: {stepIndex}, Expected: {expectedPower[pullingRiderIndex]}, Actual: {actualSteps[stepIndex].Power}");
                Assert.Equal(expectedPower[pullingRiderIndex], actualSteps[stepIndex].Power);
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
}
