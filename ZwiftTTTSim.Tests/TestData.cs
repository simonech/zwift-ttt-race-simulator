using ZwiftTTTSim.Core.Model;

namespace ZwiftTTTSim.Tests;

public static class TestData
{
    public static List<RiderPowerPlan> GetSampleRiderPowerPlans()
    {
        return new List<RiderPowerPlan>
        {
            new RiderPowerPlan
            {
                Name = "Alice",
                PullDuration = TimeSpan.FromSeconds(30),
                PowerByPosition = new[] { 350, 300, 280, 250 },
                Rider = new RiderData { FTP = 300, Weight = 70 }
            },
            new RiderPowerPlan
            {
                Name = "Bob",
                PullDuration = TimeSpan.FromSeconds(45),
                PowerByPosition = new[] { 330, 290, 270, 240 },
                Rider = new RiderData { FTP = 280, Weight = 75 },
            },
            new RiderPowerPlan
            {
                Name = "Charlie",
                PullDuration = TimeSpan.FromSeconds(60),
                PowerByPosition = new[] { 370, 320, 300, 270 },
                Rider = new RiderData { FTP = 320, Weight = 72 }
            },
            new RiderPowerPlan
            {
                Name = "Diana",
                PullDuration = TimeSpan.FromSeconds(90),
                PowerByPosition = new[] { 340, 310, 290, 260 },
                Rider = new RiderData { FTP = 290, Weight = 68 }
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
            PowerByPosition = new[] { 280, 260, 230, 200 },
            Rider = new RiderData { FTP = 260, Weight = 98 }
        });
        list.Add(new RiderPowerPlan
        {
            Name = "Eve",
            PullDuration = TimeSpan.FromSeconds(40),
            PowerByPosition = new[] { 310, 280, 250, 220 },
            Rider = new RiderData { FTP = 310, Weight = 65 }
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
            PowerByPosition = new[] { 300, 270, 240, 210 },
            Rider = new RiderData { FTP = 300, Weight = 80 }
        });
        list.Add(new RiderPowerPlan
        {
            Name = "Grace",
            PullDuration = TimeSpan.FromSeconds(50),
            PowerByPosition = new[] { 320, 290, 260, 230 },
            Rider = new RiderData { FTP = 320, Weight = 68 }
        });
        return list;
    }
}
