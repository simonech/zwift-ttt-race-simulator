namespace ZwiftTTTSim.Core.Model;

/// <summary>
/// Represents a position within a pull, including the rider, their position, and target power.
/// </summary>
public class PacelinePosition
{
    /// <summary>
    /// Gets or sets the rider associated with this position.
    /// </summary>
    public required RiderPowerPlan Rider { get; set; }
    /// <summary>
    /// Gets or sets the position of the rider in the pull (0-based).
    /// </summary>
    public int PositionInPull { get; set; }

    /// <summary>
    /// Gets or sets the target power for the rider in this position.
    /// </summary>
    public double TargetPower { get; set; }
}