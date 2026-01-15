namespace ZwiftTTTSim.Core.Model;

public class RiderPowerPlan
{
    public string Name { get; set; } = string.Empty;
    public TimeSpan PullDuration { get; set; }
    // PowerByPosition mapping:
    // [0] = power when rider is in 1st position (pulling)
    // [1] = power when in 2nd position
    // [2] = power when in 3rd position
    // [3] = power for 4th and all later positions (positions >= 4 use the last entry)
    public int[] PowerByPosition { get; set; } = [];

    public RiderData Rider { get; set; } = new RiderData();
}