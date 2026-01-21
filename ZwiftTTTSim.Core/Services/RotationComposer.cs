using ZwiftTTTSim.Core.Model;

namespace ZwiftTTTSim.Core.Services;

public class RotationComposer
{
    /// <summary>
    /// Creates a list of Pulls based on the provided RiderPowerPlans and number of rotations.
    /// Each Pull represents a rotation with riders in specific positions and their target powers.
    /// </summary>
    /// <param name="powerPlans">List of RiderPowerPlan representing each rider's power plan.</param>
    /// <param name="rotations">Number of rotations to simulate.</param>
    /// <returns>List of Pull objects representing the entire rotation sequence.</returns>
    public List<Pull> CreatePullsList(List<RiderPowerPlan> powerPlans, int rotations)
    {

        //TODO: Validate input
        // - powerPlans not empty
        // - rotations > 0
        // - each RiderPowerPlan has valid PowerByPosition data
        // - each RiderPowerPlan has valid Rider data (FTP > 0)
        // - each RiderPowerPlan has PullDuration > 0
        // - names are unique
        // Throw exceptions if any validation fails
        
        var pullsList = new List<Pull>();

        // Clone the list to maintain original order during rotations
        var rotationOrder = new List<RiderPowerPlan>(powerPlans);

        int totalRiders = rotationOrder.Count;
        int totalPulls = rotations * totalRiders;

        for (int pull = 0; pull < totalPulls; pull++)
        {
            var leader = rotationOrder[0];
            var pullDuration = leader.PullDuration;

            var currentPull = new Pull
            {
                PullNumber = pull + 1,
                PullDuration = pullDuration,
                PullPositions = new List<PullPosition>()
            };

            for (int position = 0; position < rotationOrder.Count; position++)
            {
                var currentRider = rotationOrder[position];

                var targetPower = currentRider.GetPowerByPosition(position);

                //TODO: remove SetIntensity from rotation logic and move to export logic (see comment in WorkoutStep.cs)
                //step.SetIntensity(currentRider.Rider.FTP);
                currentPull.PullPositions.Add(new PullPosition
                {
                    Rider = currentRider,
                    PositionInPull = position,
                    TargetPower = targetPower
                });
            }
            pullsList.Add(currentPull);
            // Move the leader to the end of the rotation
            rotationOrder.Add(rotationOrder[0]);
            rotationOrder.RemoveAt(0);
        }

        return pullsList;
    }
}
