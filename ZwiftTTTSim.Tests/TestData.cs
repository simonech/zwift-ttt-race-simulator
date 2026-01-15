using ZwiftTTTSim.Core.Model;

namespace ZwiftTTTSim.Tests;

public static class TestData
{
    public static List<RiderData> GetSampleRiders()
    {
        return new List<RiderData>
        {
            new RiderData { Name = "Alice", FTP = 300, Weight = 70 },
            new RiderData { Name = "Bob", FTP = 280, Weight = 75 },
            new RiderData { Name = "Charlie", FTP = 320, Weight = 72 },
            new RiderData { Name = "Diana", FTP = 290, Weight = 68 }
        };
    }

    public static List<RiderPowerPlan> GetSampleRiderPowerPlans()
    {
        return new List<RiderPowerPlan>
        {
            new RiderPowerPlan
            {
                Name = "Alice",
                PullDuration = TimeSpan.FromSeconds(30),
                PowerByPosition = new[] { 350, 300, 280, 250 }
            },
            new RiderPowerPlan
            {
                Name = "Bob",
                PullDuration = TimeSpan.FromSeconds(45),
                PowerByPosition = new[] { 330, 290, 270, 240 }
            },
            new RiderPowerPlan
            {
                Name = "Charlie",
                PullDuration = TimeSpan.FromSeconds(60),
                PowerByPosition = new[] { 370, 320, 300, 270 }
            },
            new RiderPowerPlan
            {
                Name = "Diana",
                PullDuration = TimeSpan.FromSeconds(90),
                PowerByPosition = new[] { 340, 310, 290, 260 }
            }
        };
    }
}
