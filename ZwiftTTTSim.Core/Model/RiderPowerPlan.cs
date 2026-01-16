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

    /// <summary>
    /// Gets the power value for a specific position in the rotation.
    /// Positions beyond the defined entries automatically use the last entry.
    /// </summary>
    /// <param name="position">The position index (0-based).</param>
    /// <returns>The power value for the specified position.</returns>
    public int GetPowerByPosition(int position)
    {
        return PowerByPosition[Math.Min(position, PowerByPosition.Length - 1)];
    }
}