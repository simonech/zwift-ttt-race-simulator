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
            new RiderData { Name = "Diana", FTP = 290, Weight = 68 },
            new RiderData { Name = "Simone", FTP = 260, Weight = 98 }
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

    public static List<RiderPowerPlan> GetSampleRiderPowerPlans6()
    {
        var list = GetSampleRiderPowerPlans();
        list.Add(new RiderPowerPlan
        {
            Name = "Simone",
            PullDuration = TimeSpan.FromSeconds(45),
            PowerByPosition = new[] { 280, 260, 230, 200 }
        });
        list.Add(new RiderPowerPlan
        {
            Name = "Eve",
            PullDuration = TimeSpan.FromSeconds(40),
            PowerByPosition = new[] { 310, 280, 250, 220 }
        });
        return list;
    }

    public static List<RiderPowerPlan> GetSampleRiderPowerPlans8()
    {
        var list = GetSampleRiderPowerPlans6();
        list.Add(new RiderPowerPlan
        {
            Name = "Frank",
            PullDuration = TimeSpan.FromSeconds(35),
            PowerByPosition = new[] { 300, 270, 240, 210 }
        });
        list.Add(new RiderPowerPlan
        {
            Name = "Grace",
            PullDuration = TimeSpan.FromSeconds(50),
            PowerByPosition = new[] { 320, 290, 260, 230 }
        });
        return list;
    }
}
