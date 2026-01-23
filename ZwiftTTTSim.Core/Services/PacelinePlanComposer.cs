using ZwiftTTTSim.Core.Model;

namespace ZwiftTTTSim.Core.Services;

public class PacelinePlanComposer
{
    /// <summary>
    /// Creates a PacelinePlan based on the provided rider power plans and number of rotations.
    /// </summary>
    /// <param name="powerPlans">List of RiderPowerPlan objects representing each rider's power plan.</param>
    /// <param name="rotations">Number of rotations to simulate.</param>
    /// <returns>PacelinePlan object representing the entire race plan.</returns>
    public PacelinePlan CreatePlan(List<RiderPowerPlan> powerPlans, int rotations)
    {

        //TODO: Validate input
        // - powerPlans not empty
        // - rotations > 0
        // - each RiderPowerPlan has valid PowerByPosition data
        // - each RiderPowerPlan has valid Rider data (FTP > 0)
        // - each RiderPowerPlan has PullDuration > 0
        // - names are unique
        // Throw exceptions if any validation fails
        
        var plan = new PacelinePlan();

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
                PacelinePositions = new List<PacelinePosition>()
            };

            for (int position = 0; position < rotationOrder.Count; position++)
            {
                var currentRider = rotationOrder[position];

                var targetPower = currentRider.GetPowerByPosition(position);

                currentPull.PacelinePositions.Add(new PacelinePosition
                {
                    Rider = currentRider,
                    PositionInPull = position,
                    TargetPower = targetPower
                });
            }
            plan.Pulls.Add(currentPull);
            // Move the leader to the end of the rotation
            rotationOrder.Add(rotationOrder[0]);
            rotationOrder.RemoveAt(0);
        }

        return plan;
    }
}
