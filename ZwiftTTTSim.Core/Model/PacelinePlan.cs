using ZwiftTTTSim.Core.Model;

/// <summary>
/// Represents the complete paceline rotation plan for a team, including
/// the sequence of pulls and the aggregate duration of the plan.
/// </summary>
public class PacelinePlan
{
    /// <summary>
/// Gets the ordered list of <see cref="Pull"/> instances that make up
/// the paceline rotation plan.
/// </summary>
    public List<Pull> Pulls { get; init; } = new List<Pull>();

    /// <summary>
/// Gets the total planned duration of the paceline by summing the
/// <see cref="Pull.PullDuration"/> of all pulls in <see cref="Pulls"/>.
/// </summary>
    public TimeSpan TotalDuration => Pulls.Aggregate(TimeSpan.Zero, (total, pull) => total + pull.PullDuration);
}