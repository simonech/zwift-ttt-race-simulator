using ZwiftTTTSim.Core.Model;

namespace ZwiftTTTSim.Core.Services;

public class WorkoutCreator
{
    public Dictionary<string, List<WorkoutStep>> CreateWorkouts(List<RiderPowerPlan> powerPlans, int rotations)
    {

        //TODO: Validate input
        
        var workouts = powerPlans.ToDictionary(
            plan => plan.Name,
            _ => new List<WorkoutStep>()
        );

        // Clone the list to maintain original order during rotations
        var rotationOrder = new List<RiderPowerPlan>(powerPlans);

        int totalRiders = rotationOrder.Count;
        int totalPulls = rotations * totalRiders;

        for (int pull = 0; pull < totalPulls; pull++)
        {
            var leader = rotationOrder[0];
            var pullDurationSeconds = (int)leader.PullDuration.TotalSeconds;

            for (int position = 0; position < rotationOrder.Count; position++)
            {
                var currentRider = rotationOrder[position];

                var targetPower = currentRider.GetPowerByPosition(position);

                var step = new WorkoutStep
                {
                    DurationSeconds = pullDurationSeconds,
                    Power = targetPower
                };
                //TODO: remove SetIntensity from rotation logic and move to export logic
                step.SetIntensity(currentRider.Rider.FTP);
                workouts[currentRider.Name].Add(step);
            }
            rotationOrder.Add(rotationOrder[0]);
            rotationOrder.RemoveAt(0);
        }

        return workouts;
    }
}
