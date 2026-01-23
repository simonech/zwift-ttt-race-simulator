using ZwiftTTTSim.Core.Model;

namespace ZwiftTTTSim.Core.Services;

/// <summary>
/// Projects a <see cref="PacelinePlan"/> into per-rider workout step sequences.
/// </summary>
public class WorkoutProjector
{

    /// <summary>
    /// Projects the specified paceline plan into a collection of workouts,
    /// grouped by rider name, where each workout is represented as an ordered
    /// list of <see cref="WorkoutStep"/> instances.
    /// </summary>
    /// <param name="plan">The paceline plan containing pulls and rider positions to project into workouts.</param>
    /// <returns>
/// A dictionary keyed by rider name, where each value is the list of
/// <see cref="WorkoutStep"/> objects representing that rider's workout.
/// </returns>
/// <exception cref="ArgumentNullException"><paramref name="plan"/> is <c>null</c>.</exception>
    public Dictionary<string, List<WorkoutStep>> Project(PacelinePlan plan)
    {

        if (plan == null)
        {
            throw new ArgumentNullException(nameof(plan));
        }

        var workouts = new Dictionary<string, List<WorkoutStep>>();

        foreach (var pull in plan.Pulls)
        {
            foreach (var position in pull.PacelinePositions)
            {
                if (!workouts.ContainsKey(position.Rider.Name))
                {
                    workouts[position.Rider.Name] = new List<WorkoutStep>();
                }

                var step = new WorkoutStep
                {
                    DurationSeconds = (int)pull.PullDuration.TotalSeconds,
                    Power = position.TargetPower
                };
                step.SetIntensity(position.Rider.RiderData.FTP);

                workouts[position.Rider.Name].Add(step);
            }
        }

        return workouts;
    }

}