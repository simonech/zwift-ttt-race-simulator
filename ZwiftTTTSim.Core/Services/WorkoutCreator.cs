using ZwiftTTTSim.Core.Model;

namespace ZwiftTTTSim.Core.Services;

public class WorkoutCreator
{
    public Dictionary<string, List<WorkoutStep>> CreateWorkouts(List<RiderPowerPlan> powerPlans, int rotations)
    {
        var workouts = new Dictionary<string, List<WorkoutStep>>();

        foreach (var riderPlan in powerPlans)
        {
            var steps = new List<WorkoutStep>();
            workouts[riderPlan.Name] = steps;
        }
        int totalRiders = powerPlans.Count;
        for (int rotation = 0; rotation < rotations * totalRiders; rotation++)
        {
            var durationSeconds = (int)powerPlans[0].PullDuration.TotalSeconds;

            for (int i = 0; i < powerPlans.Count; i++)
            {
                var riderName = powerPlans[i].Name;

                var step = new WorkoutStep
                {
                    DurationSeconds = durationSeconds,
                    Power = powerPlans[i].PowerByPosition[i]
                };
                workouts[riderName].Add(step);
            }
            powerPlans.Add(powerPlans[0]);
            powerPlans.RemoveAt(0);
        }

        return workouts;
    }
}
