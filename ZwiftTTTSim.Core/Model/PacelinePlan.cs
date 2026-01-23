using ZwiftTTTSim.Core.Model;

public class PacelinePlan
{
    public List<Pull> Pulls { get; init; } = new List<Pull>();

    public TimeSpan TotalDuration => Pulls.Aggregate(TimeSpan.Zero, (total, pull) => total + pull.PullDuration);
}