using Xunit;
using ZwiftTTTSim.Core.Helpers;
using ZwiftTTTSim.Core.Model;
using ZwiftTTTSim.Core.Services;

namespace ZwiftTTTSim.Tests;

public class WorkoutCreatorTests
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

    [Theory]
    [MemberData(nameof(PowerPlanSets))]
    public void WorkoutCreator_ShouldAlwaysCreateSameNumberOfWorkoutsAsNumberOfRiders(List<RiderPowerPlan> riders, int rotations)
    {
        // Arrange
        var workoutCreator = new WorkoutCreator();

        // Act
        var workouts = workoutCreator.CreateWorkouts(riders, rotations);

        // Assert
        Assert.Equal(riders.Count, workouts.Count);
    }

    [Theory]
    [MemberData(nameof(PowerPlanSets))]
    public void WorkoutCreator_ShouldCreateNumberOfPullsEqualToRidersTimesRotations(List<RiderPowerPlan> riders, int rotations)
    {
        // Arrange
        var workoutCreator = new WorkoutCreator();
        var expectedPullsCount = riders.Count * rotations;
        // Act
        var workouts = workoutCreator.CreateWorkouts(riders, rotations);

        // Assert
        foreach (var workout in workouts.Values)
        {
            Assert.Equal(expectedPullsCount, workout.Count);
        }
    }

    [Theory]
    [MemberData(nameof(PowerPlanSets))]
    public void WorkoutCreator_ShouldHaveCorrectPowerValuesForEachRider(List<RiderPowerPlan> riders, int rotations)
    {
        // Arrange
        var workoutCreator = new WorkoutCreator();

        // Act
        var workouts = workoutCreator.CreateWorkouts(riders, rotations);

        // Assert - Verify each rider's power values are correct
        for (int riderIndex = 0; riderIndex < riders.Count; riderIndex++)
        {
            var riderName = riders[riderIndex].Name;
            var actualSteps = workouts[riderName];

            for (int stepIndex = 0; stepIndex < actualSteps.Count; stepIndex++)
            {
                var pullingRiderIndex = stepIndex % riders.Count;
                // Compute rider's position at this step relative to the pulling rider
                var position = Helpers.GetRotationPosition(riderIndex, pullingRiderIndex, riders.Count);
                var expectedPower = riders[riderIndex].GetPowerByPosition(position);

                Assert.Equal(expectedPower, actualSteps[stepIndex].Power);
            }
        }
    }

    [Theory]
    [MemberData(nameof(PowerPlanSets))]
    public void WorkoutCreator_ShouldHaveCorrectDurationValuesForEachStep(List<RiderPowerPlan> riders, int rotations)
    {
        // Arrange
        var workoutCreator = new WorkoutCreator();

        // Act
        var workouts = workoutCreator.CreateWorkouts(riders, rotations);

        // Assert - Verify each step has the correct duration based on the pulling rider
        for (int riderIndex = 0; riderIndex < riders.Count; riderIndex++)
        {
            var riderName = riders[riderIndex].Name;
            var actualSteps = workouts[riderName];

            for (int stepIndex = 0; stepIndex < actualSteps.Count; stepIndex++)
            {
                var pullingRiderIndex = stepIndex % riders.Count;
                var expectedDurationSeconds = (int)riders[pullingRiderIndex].PullDuration.TotalSeconds;

                Assert.Equal(expectedDurationSeconds, actualSteps[stepIndex].DurationSeconds);
            }
        }
    }

    [Fact]
    public void WorkoutCreator_SampleTest_FourRidersOneRotation_ShouldGenerateCorrectWorkout()
    {
        // Arrange - A 4-rider team with one complete rotation
        var powerPlans = new List<RiderPowerPlan>
        {
            new RiderPowerPlan
            {
                Name = "Alice",
                PullDuration = TimeSpan.FromSeconds(30),
                PowerByPosition = [350, 300, 250, 200],
                Rider = new RiderData { FTP = 300, Weight = 70 }
            },
            new RiderPowerPlan
            {
                Name = "Bob",
                PullDuration = TimeSpan.FromSeconds(30),
                PowerByPosition = [350, 300, 250, 200],
                Rider = new RiderData { FTP = 280, Weight = 75 }
            },
            new RiderPowerPlan
            {
                Name = "Charlie",
                PullDuration = TimeSpan.FromSeconds(30),
                PowerByPosition = [350, 300, 250, 200],
                Rider = new RiderData { FTP = 320, Weight = 72 }
            },
            new RiderPowerPlan
            {
                Name = "Diana",
                PullDuration = TimeSpan.FromSeconds(30),
                PowerByPosition = [350, 300, 250, 200],
                Rider = new RiderData { FTP = 290, Weight = 68 }
            }
        };

        var workoutCreator = new WorkoutCreator();

        // Act
        var workouts = workoutCreator.CreateWorkouts(powerPlans, rotations: 1);

        // Assert
        Assert.Equal(4, workouts.Count);
        foreach (var workout in workouts.Values)
        {
            Assert.Equal(4, workout.Count); // 4 riders × 1 rotation = 4 pulls
        }

        // Alice's workout: pulls at position 0,3,2,1
        var aliceSteps = workouts["Alice"];
        Assert.Equal(350, aliceSteps[0].Power); // Position 0 (pulling)
        Assert.Equal(200, aliceSteps[1].Power); // Position 3 (last person)
        Assert.Equal(250, aliceSteps[2].Power); // Position 2
        Assert.Equal(300, aliceSteps[3].Power); // Position 1

        // All steps should have the same duration when all riders have same pull duration
        foreach (var step in aliceSteps)
        {
            Assert.Equal(30, step.DurationSeconds);
        }
    }

    [Fact]
    public void WorkoutCreator_SampleTest_SixRidersWithVariableDurations_ShouldTrackPullingRiderDuration()
    {
        // Arrange - A 6-rider team with different pull durations
        var powerPlans = new List<RiderPowerPlan>
        {
            new RiderPowerPlan
            {
                Name = "Fast",
                PullDuration = TimeSpan.FromSeconds(20),
                PowerByPosition = [400, 300, 250, 200],
                Rider = new RiderData { FTP = 350, Weight = 65 }
            },
            new RiderPowerPlan
            {
                Name = "Steady",
                PullDuration = TimeSpan.FromSeconds(40),
                PowerByPosition = [350, 290, 240, 190],
                Rider = new RiderData { FTP = 300, Weight = 75 }
            },
            new RiderPowerPlan
            {
                Name = "Strong",
                PullDuration = TimeSpan.FromSeconds(60),
                PowerByPosition = [380, 320, 270, 210],
                Rider = new RiderData { FTP = 340, Weight = 80 }
            }
        };

        var workoutCreator = new WorkoutCreator();

        // Act
        var workouts = workoutCreator.CreateWorkouts(powerPlans, rotations: 1);

        // Assert - Duration should match the pulling rider
        var fastSteps = workouts["Fast"];
        var steadySteps = workouts["Steady"];
        var strongSteps = workouts["Strong"];

        // When Fast is pulling (step 0), duration is 20s
        Assert.Equal(20, fastSteps[0].DurationSeconds);
        Assert.Equal(20, steadySteps[0].DurationSeconds);
        Assert.Equal(20, strongSteps[0].DurationSeconds);

        // When Steady is pulling (step 1), duration is 40s
        Assert.Equal(40, fastSteps[1].DurationSeconds);
        Assert.Equal(40, steadySteps[1].DurationSeconds);
        Assert.Equal(40, strongSteps[1].DurationSeconds);

        // When Strong is pulling (step 2), duration is 60s
        Assert.Equal(60, fastSteps[2].DurationSeconds);
        Assert.Equal(60, steadySteps[2].DurationSeconds);
        Assert.Equal(60, strongSteps[2].DurationSeconds);
    }

    [Fact]
    public void WorkoutCreator_SampleTest_RiderWithMoreThanFourPositions_ShouldClampToLastPowerValue()
    {
        // Arrange - 5 riders where position 4+ should use the 4th power value
        var powerPlans = new List<RiderPowerPlan>
        {
            new RiderPowerPlan
            {
                Name = "Leader",
                PullDuration = TimeSpan.FromSeconds(30),
                PowerByPosition = [400, 350, 300, 250], // Only 4 positions defined
                Rider = new RiderData { FTP = 310, Weight = 72 }
            },
            new RiderPowerPlan
            {
                Name = "Second",
                PullDuration = TimeSpan.FromSeconds(30),
                PowerByPosition = [400, 350, 300, 250],
                Rider = new RiderData { FTP = 305, Weight = 73 }
            },
            new RiderPowerPlan
            {
                Name = "Third",
                PullDuration = TimeSpan.FromSeconds(30),
                PowerByPosition = [400, 350, 300, 250],
                Rider = new RiderData { FTP = 300, Weight = 74 }
            },
            new RiderPowerPlan
            {
                Name = "Fourth",
                PullDuration = TimeSpan.FromSeconds(30),
                PowerByPosition = [400, 350, 300, 250],
                Rider = new RiderData { FTP = 295, Weight = 75 }
            },
            new RiderPowerPlan
            {
                Name = "Fifth",
                PullDuration = TimeSpan.FromSeconds(30),
                PowerByPosition = [400, 350, 300, 250],
                Rider = new RiderData { FTP = 290, Weight = 76 }
            }
        };

        var workoutCreator = new WorkoutCreator();

        // Act
        var workouts = workoutCreator.CreateWorkouts(powerPlans, rotations: 1);

        // Assert
        var fifthSteps = workouts["Fifth"];
        
        // Fifth rider's positions during rotation: 4,3,2,1,0
        // Position 4 should clamp to the last value (250W)
        Assert.Equal(250, fifthSteps[0].Power);
        // Position 3 should use index 3 (250W)
        Assert.Equal(250, fifthSteps[1].Power);
        // Position 2 should use index 2 (300W)
        Assert.Equal(300, fifthSteps[2].Power);
        // Position 1 should use index 1 (350W)
        Assert.Equal(350, fifthSteps[3].Power);
        // Position 0 should use index 0 (400W)
        Assert.Equal(400, fifthSteps[4].Power);
    }

    [Fact]
    public void WorkoutCreator_SampleTest_ThreeRidersTwoRotations_ShouldRepeatCycleCorrectly()
    {
        // Arrange - A 3-rider team completing 2 full rotations (6 total pulls)
        var powerPlans = new List<RiderPowerPlan>
        {
            new RiderPowerPlan
            {
                Name = "Alice",
                PullDuration = TimeSpan.FromSeconds(30),
                PowerByPosition = [380, 320, 260],
                Rider = new RiderData { FTP = 300, Weight = 70 }
            },
            new RiderPowerPlan
            {
                Name = "Bob",
                PullDuration = TimeSpan.FromSeconds(45),
                PowerByPosition = [360, 300, 240],
                Rider = new RiderData { FTP = 280, Weight = 75 }
            },
            new RiderPowerPlan
            {
                Name = "Charlie",
                PullDuration = TimeSpan.FromSeconds(60),
                PowerByPosition = [400, 340, 280],
                Rider = new RiderData { FTP = 320, Weight = 72 }
            }
        };

        var workoutCreator = new WorkoutCreator();

        // Act
        var workouts = workoutCreator.CreateWorkouts(powerPlans, rotations: 2);

        // Assert
        Assert.Equal(3, workouts.Count);
        foreach (var workout in workouts.Values)
        {
            Assert.Equal(6, workout.Count); // 3 riders × 2 rotations = 6 pulls
        }

        var aliceSteps = workouts["Alice"];
        var bobSteps = workouts["Bob"];
        var charlieSteps = workouts["Charlie"];

        // First rotation (pulls 0-2)
        // Pull 0: Alice pulls (30s for everyone)
        Assert.Equal(30, aliceSteps[0].DurationSeconds);
        Assert.Equal(30, bobSteps[0].DurationSeconds);
        Assert.Equal(30, charlieSteps[0].DurationSeconds);

        // Pull 1: Bob pulls (45s for everyone)
        Assert.Equal(45, aliceSteps[1].DurationSeconds);
        Assert.Equal(45, bobSteps[1].DurationSeconds);
        Assert.Equal(45, charlieSteps[1].DurationSeconds);

        // Pull 2: Charlie pulls (60s for everyone)
        Assert.Equal(60, aliceSteps[2].DurationSeconds);
        Assert.Equal(60, bobSteps[2].DurationSeconds);
        Assert.Equal(60, charlieSteps[2].DurationSeconds);

        // Second rotation (pulls 3-5) - should repeat the same pattern
        // Pull 3: Alice pulls (30s for everyone)
        Assert.Equal(30, aliceSteps[3].DurationSeconds);
        Assert.Equal(30, bobSteps[3].DurationSeconds);
        Assert.Equal(30, charlieSteps[3].DurationSeconds);

        // Pull 4: Bob pulls (45s for everyone)
        Assert.Equal(45, aliceSteps[4].DurationSeconds);
        Assert.Equal(45, bobSteps[4].DurationSeconds);
        Assert.Equal(45, charlieSteps[4].DurationSeconds);

        // Pull 5: Charlie pulls (60s for everyone)
        Assert.Equal(60, aliceSteps[5].DurationSeconds);
        Assert.Equal(60, bobSteps[5].DurationSeconds);
        Assert.Equal(60, charlieSteps[5].DurationSeconds);

        // Verify power values also repeat correctly
        // Alice: position 0,2,1,0,2,1 across the 6 pulls
        Assert.Equal(380, aliceSteps[0].Power); // pos 0
        Assert.Equal(260, aliceSteps[1].Power); // pos 2
        Assert.Equal(320, aliceSteps[2].Power); // pos 1
        Assert.Equal(380, aliceSteps[3].Power); // pos 0 (repeats)
        Assert.Equal(260, aliceSteps[4].Power); // pos 2
        Assert.Equal(320, aliceSteps[5].Power); // pos 1
    }
}
