namespace ZwiftTTTSim.Core.Model;

/// <summary>
/// Represents a rider's power plan for different positions in a team time trial simulation.
/// </summary>
public class RiderPowerPlan
{
    /// <summary>
    /// Gets or sets the name of the rider.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the duration of each pull in the rotation.
    /// </summary>
    public TimeSpan PullDuration { get; set; }
    // PowerByPosition mapping:
    // [0] = power when rider is in 1st position (pulling)
    // [1] = power when in 2nd position
    // [2] = power when in 3rd position
    // [3] = power for 4th and all later positions (positions >= 4 use the last entry)
    public int[] PowerByPosition { get; set; } = [];

    /// <summary>
    /// Gets or sets the rider's essential data.
    /// </summary>
    public RiderData RiderData { get; set; } = new RiderData();

    /// <summary>
    /// Gets the power value for a specific position in the rotation.
    /// Positions beyond the defined entries automatically use the last entry.
    /// </summary>
    /// <param name="position">The position index (0-based).</param>
    /// <returns>The power value for the specified position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when position is negative.</exception>
    /// <exception cref="InvalidOperationException">Thrown when PowerByPosition is null or empty.</exception>
    public int GetPowerByPosition(int position)
    {
        if (position < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(position), "Position must be non-negative.");
        }

        if (PowerByPosition == null || PowerByPosition.Length == 0)
        {
            throw new InvalidOperationException("There is no data in the PowerByPosition array.");
        }
        return PowerByPosition[Math.Min(position, PowerByPosition.Length - 1)];
    }
}