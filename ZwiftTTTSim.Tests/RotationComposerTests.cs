using Xunit;
using ZwiftTTTSim.Core.Model;
using ZwiftTTTSim.Core.Services;

namespace ZwiftTTTSim.Tests;

public class RotationComposerTests
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
    public void RotationComposer_ShouldAlwaysCreateSameNumberOfPullsPositionsAsNumberOfRiders(List<RiderPowerPlan> riders, int rotations)
    {
        // Arrange
        var rotationComposer = new RotationComposer();

        // Act
        var pulls = rotationComposer.CreatePullsList(riders, rotations);

        // Assert
        foreach (var pull in pulls)
        {
            Assert.Equal(riders.Count, pull.PullPositions.Count);
        }
    }

    [Theory]
    [MemberData(nameof(PowerPlanSets))]
    public void RotationComposer_ShouldCreateNumberOfPullsEqualToRidersTimesRotations(List<RiderPowerPlan> riders, int rotations)
    {
        // Arrange
        var rotationComposer = new RotationComposer();
        var expectedPullsCount = riders.Count * rotations;
        // Act
        var pulls = rotationComposer.CreatePullsList(riders, rotations);

        // Assert

        Assert.Equal(expectedPullsCount, pulls.Count);

    }

    [Theory]
    [MemberData(nameof(PowerPlanSets))]
    public void RotationComposer_ShouldHaveCorrectPowerValuesForEachRider(List<RiderPowerPlan> riders, int rotations)
    {
        // Arrange
        var rotationComposer = new RotationComposer();

        // Act
        var pulls = rotationComposer.CreatePullsList(riders, rotations);

        // Assert - Verify each rider's power values are correct
        for (int pullIndex = 0; pullIndex < pulls.Count; pullIndex++)
        {
            var pull = pulls[pullIndex];

            var pullPositions = pull.PullPositions;

            foreach (var currentPull in pullPositions)
            {
                var currentRider = currentPull.Rider;
                var expectedPower = currentRider.GetPowerByPosition(currentPull.PositionInPull);
                Assert.Equal(expectedPower, currentPull.TargetPower);
            }
        }
    }

    [Theory]
    [MemberData(nameof(PowerPlanSets))]
    public void RotationComposer_ShouldHaveCorrectDurationValuesForEachPull(List<RiderPowerPlan> riders, int rotations)
    {
        // Arrange
        var rotationComposer = new RotationComposer();

        // Act
        var pulls = rotationComposer.CreatePullsList(riders, rotations);

        // Assert - Verify each step has the correct duration based on the pulling rider
        for (int pullIndex = 0; pullIndex < pulls.Count; pullIndex++)
        {
            var pull = pulls[pullIndex];

            // Determine which rider is pulling in this pull
            var pullingRiderIndex = pullIndex % riders.Count;
            var expectedDurationSeconds = (int)riders[pullingRiderIndex].PullDuration.TotalSeconds;

            Assert.Equal(expectedDurationSeconds, pull.PullDuration.TotalSeconds);
        }
    }

    [Fact]
    public void RotationComposer_SampleTest_FourRidersOneRotation_ShouldGeneratePulls()
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

        var rotationComposer = new RotationComposer();

        // Act
        var pullList = rotationComposer.CreatePullsList(powerPlans, rotations: 1);

        // Assert
        Assert.Equal(4, pullList.Count);
        foreach (var pull in pullList)
        {
            Assert.Equal(4, pull.PullPositions.Count); // 4 riders × 1 rotation = 4 pulls
        }
        // Verify specific power values and durations
        // Pull 1: Alice pulls
        Assert.Equal(350, pullList[0].PullPositions[0].TargetPower); // Alice position 0
        Assert.Equal(300, pullList[0].PullPositions[1].TargetPower); // Bob position 1
        Assert.Equal(250, pullList[0].PullPositions[2].TargetPower); // Charlie position 2
        Assert.Equal(200, pullList[0].PullPositions[3].TargetPower); // Diana position 3

        // Pull 2: Bob pulls
        Assert.Equal(350, pullList[1].PullPositions[0].TargetPower); // Bob position 0
        Assert.Equal(300, pullList[1].PullPositions[1].TargetPower); // Charlie position 1
        Assert.Equal(250, pullList[1].PullPositions[2].TargetPower); // Diana position 2
        Assert.Equal(200, pullList[1].PullPositions[3].TargetPower); // Alice position 3

        // Pull 3: Charlie pulls
        Assert.Equal(350, pullList[2].PullPositions[0].TargetPower); // Charlie position 0
        Assert.Equal(300, pullList[2].PullPositions[1].TargetPower); // Diana position 1
        Assert.Equal(250, pullList[2].PullPositions[2].TargetPower); // Alice position 2
        Assert.Equal(200, pullList[2].PullPositions[3].TargetPower); // Bob position 3

        // Pull 4: Diana pulls
        Assert.Equal(350, pullList[3].PullPositions[0].TargetPower); // Diana position 0
        Assert.Equal(300, pullList[3].PullPositions[1].TargetPower); // Alice position 1
        Assert.Equal(250, pullList[3].PullPositions[2].TargetPower); // Bob position 2
        Assert.Equal(200, pullList[3].PullPositions[3].TargetPower); // Charlie position 3

        // All pulls should have duration of 30 seconds
        foreach (var pull in pullList)
        {
            Assert.Equal(TimeSpan.FromSeconds(30), pull.PullDuration); 
        }
    }

    [Fact]
    public void RotationComposer_SampleTest_SixRidersWithVariableDurations_ShouldTrackPullingRiderDuration()
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
            },
            new RiderPowerPlan
            {
                Name = "Fast1",
                PullDuration = TimeSpan.FromSeconds(20),
                PowerByPosition = [400, 300, 250, 200],
                Rider = new RiderData { FTP = 350, Weight = 65 }
            },
            new RiderPowerPlan
            {
                Name = "Steady1",
                PullDuration = TimeSpan.FromSeconds(40),
                PowerByPosition = [350, 290, 240, 190],
                Rider = new RiderData { FTP = 300, Weight = 75 }
            },
            new RiderPowerPlan
            {
                Name = "Strong1",
                PullDuration = TimeSpan.FromSeconds(60),
                PowerByPosition = [380, 320, 270, 210],
                Rider = new RiderData { FTP = 340, Weight = 80 }
            }
        };

        var rotationComposer = new RotationComposer();

        // Act
        var pullList = rotationComposer.CreatePullsList(powerPlans, rotations: 1);

        // Assert - Duration should match the pulling rider

        // When Fast is pulling (step 0), duration is 20s
        Assert.Equal(TimeSpan.FromSeconds(20), pullList[0].PullDuration);
        // When Steady is pulling (step 1), duration is 40s
        Assert.Equal(TimeSpan.FromSeconds(40), pullList[1].PullDuration);
        // When Strong is pulling (step 2), duration is 60s
        Assert.Equal(TimeSpan.FromSeconds(60), pullList[2].PullDuration);
        // When Fast1 is pulling (step 3), duration is 20s
        Assert.Equal(TimeSpan.FromSeconds(20), pullList[3].PullDuration);
        // When Steady is pulling (step 4), duration is 40s
        Assert.Equal(TimeSpan.FromSeconds(40), pullList[4].PullDuration);
        // When Strong1 is pulling (step 5), duration is 60s
        Assert.Equal(TimeSpan.FromSeconds(60), pullList[5].PullDuration);
    }

    [Fact]
    public void RotationComposer_SampleTest_RiderWithMoreThanFourPositions_ShouldClampToLastPowerValue()
    {
        // Arrange - 5 riders where position 4+ should use the 4th power value
        var powerPlans = new List<RiderPowerPlan>
        {
            new RiderPowerPlan
            {
                Name = "Leader",
                PullDuration = TimeSpan.FromSeconds(30),
                PowerByPosition = [400, 350, 300, 250], 
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

        var rotationComposer = new RotationComposer();

        // Act
        var pullList = rotationComposer.CreatePullsList(powerPlans, rotations: 1);

        // Assert
        // In Pull 1, Fifth rider is in position 4, should use last power value (250)
        Assert.Equal(250, pullList[0].PullPositions[4].TargetPower);
        // In Pull 2, Fourth rider is in position 4, should use last power value (250)
        Assert.Equal(250, pullList[1].PullPositions[4].TargetPower);
        // In Pull 3, Third rider is in position 4, should use last power value (250)
        Assert.Equal(250, pullList[2].PullPositions[4].TargetPower);
        // In Pull 4, Second rider is in position 4, should use last power value (250)
        Assert.Equal(250, pullList[3].PullPositions[4].TargetPower);
        // In Pull 5, Leader rider is in position 4, should use last power value (250)
        Assert.Equal(250, pullList[4].PullPositions[4].TargetPower);
    }

    [Fact]
    public void RotationComposer_SampleTest_ThreeRidersTwoRotations_ShouldRepeatCycleCorrectly()
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

        var rotationComposer = new RotationComposer();

        // Act
        var pullList = rotationComposer.CreatePullsList(powerPlans, rotations: 2);

        // Assert
        // Total pulls should be 6 (3 riders × 2 rotations)
        Assert.Equal(6, pullList.Count);
        foreach (var pull in pullList)
        {
            Assert.Equal(3, pull.PullPositions.Count); // 3 riders
        }


        // First rotation (pulls 0-2)
        // Pull 0: Alice pulls (30s for everyone)
        Assert.Equal(30, pullList[0].PullDuration.TotalSeconds);


        // Pull 1: Bob pulls (45s for everyone)
        Assert.Equal(45, pullList[1].PullDuration.TotalSeconds);


        // Pull 2: Charlie pulls (60s for everyone)
        Assert.Equal(60, pullList[2].PullDuration.TotalSeconds);


        // Second rotation (pulls 3-5) - should repeat the same pattern
        // Pull 3: Alice pulls (30s for everyone)
        Assert.Equal(30, pullList[3].PullDuration.TotalSeconds);


        // Pull 4: Bob pulls (45s for everyone)
        Assert.Equal(45, pullList[4].PullDuration.TotalSeconds);

        // Pull 5: Charlie pulls (60s for everyone)
        Assert.Equal(60, pullList[5].PullDuration.TotalSeconds);

        // Verify power values also repeat correctly
        // Alice: position 0,2,1,0,2,1 across the 6 pulls
        Assert.Equal(380, pullList[0].PullPositions[0].TargetPower); // pos 0
        Assert.Equal(260, pullList[1].PullPositions[2].TargetPower); // pos 2
        Assert.Equal(320, pullList[2].PullPositions[1].TargetPower); // pos 1
        Assert.Equal(380, pullList[3].PullPositions[0].TargetPower); // pos 0 (repeats)
        Assert.Equal(260, pullList[4].PullPositions[2].TargetPower); // pos 2
        Assert.Equal(320, pullList[5].PullPositions[1].TargetPower); // pos 1
    }
}
