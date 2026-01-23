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
                PowerByPosition = [ 350, 300, 280, 250 ],
                RiderData = new RiderData { FTP = 300, Weight = 70 }
            },
            new RiderPowerPlan
            {
                Name = "Bob",
                PullDuration = TimeSpan.FromSeconds(45),
                PowerByPosition = [ 330, 290, 270, 240 ],
                RiderData = new RiderData { FTP = 280, Weight = 75 },
            },
            new RiderPowerPlan
            {
                Name = "Charlie",
                PullDuration = TimeSpan.FromSeconds(60),
                PowerByPosition = [ 370, 320, 300, 270 ],
                RiderData = new RiderData { FTP = 320, Weight = 72 }
            },
            new RiderPowerPlan
            {
                Name = "Diana",
                PullDuration = TimeSpan.FromSeconds(90),
                PowerByPosition = [ 340, 310, 290, 260 ],
                RiderData = new RiderData { FTP = 290, Weight = 68 }
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
            PowerByPosition = [280, 260, 230, 200],
            RiderData = new RiderData { FTP = 260, Weight = 98 }
        });
        list.Add(new RiderPowerPlan
        {
            Name = "Eve",
            PullDuration = TimeSpan.FromSeconds(40),
            PowerByPosition = [310, 280, 250, 220],
            RiderData = new RiderData { FTP = 310, Weight = 65 }
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
            PowerByPosition = [300, 270, 240, 210],
            RiderData = new RiderData { FTP = 300, Weight = 80 }
        });
        list.Add(new RiderPowerPlan
        {
            Name = "Grace",
            PullDuration = TimeSpan.FromSeconds(50),
            PowerByPosition = [320, 290, 260, 230],
            RiderData = new RiderData { FTP = 320, Weight = 68 }
        });
        return list;
    }
}
