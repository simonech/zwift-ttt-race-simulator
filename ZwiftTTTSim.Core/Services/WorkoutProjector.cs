using ZwiftTTTSim.Core.Model;

namespace ZwiftTTTSim.Core.Services;

public class WorkoutProjector
{

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