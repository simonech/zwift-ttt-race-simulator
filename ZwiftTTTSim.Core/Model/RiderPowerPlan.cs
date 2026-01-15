namespace ZwiftTTTSim.Core.Model;

public class RiderPowerPlan
{
    public string Name { get; set; } = string.Empty;
    public TimeSpan PullDuration { get; set; }
    // PowerByPosition[0] = power for first rider, PowerByPosition[1] = power for second rider, etc.
    public int[] PowerByPosition { get; set; } = [];
}