using System;
using System.Collections.Generic;
using System.Linq;
using ZwiftTTTSim.Core.Model;
using ZwiftTTTSim.Core.Model.Segments;

namespace ZwiftTTTSim.Core.Services;

public class PacelinePlanComposer
{
    private const double DefaultClimbWkg = 4.0;

    /// <summary>
    /// Creates a PacelinePlan based on the provided rider power plans and a target route of segments.
    /// Supports dynamic rotation calculation and mixed segment pacing logic.
    /// </summary>
    /// <param name="powerPlans">List of RiderPowerPlan objects representing each rider's power plan.</param>
    /// <param name="route">List of ISegment objects making up the course.</param>
    /// <returns>PacelinePlan object representing the entire race plan.</returns>
    public PacelinePlan CreatePlan(List<RiderPowerPlan> powerPlans, List<ISegment> route)
    {
        var plan = new PacelinePlan();
        var rotationOrder = new List<RiderPowerPlan>(powerPlans);

        foreach (var segment in route)
        {
            if (segment is FlatSegment flatSegment)
            {
                var rotationDurationSeconds = powerPlans.Sum(p => p.PullDuration.TotalSeconds);
                var segmentDurationSeconds = flatSegment.Duration.TotalSeconds;

                // Calculate rotations with rounding away from zero
                var rawRotations = segmentDurationSeconds / rotationDurationSeconds;
                var rotations = (int)Math.Round(rawRotations, MidpointRounding.AwayFromZero);

                // Ensure a minimum of 1 rotation
                if (rotations < 1)
                {
                    rotations = 1;
                }

                int totalPulls = rotations * rotationOrder.Count;

                for (int pull = 0; pull < totalPulls; pull++)
                {
                    var leader = rotationOrder[0];
                    var pullDuration = leader.PullDuration;

                    var currentPull = new Pull
                    {
                        PullNumber = plan.Pulls.Count + 1,
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
                    
                    rotationOrder.Add(rotationOrder[0]);
                    rotationOrder.RemoveAt(0);
                }
            }
            else if (segment is ClimbSegment climbSegment)
            {
                var climbPull = new Pull
                {
                    PullNumber = plan.Pulls.Count + 1,
                    PullDuration = climbSegment.Duration,
                    PacelinePositions = new List<PacelinePosition>()
                };

                for (int position = 0; position < rotationOrder.Count; position++)
                {
                    var currentRider = rotationOrder[position];
                    var targetPower = currentRider.RiderData.Weight * DefaultClimbWkg;

                    climbPull.PacelinePositions.Add(new PacelinePosition
                    {
                        Rider = currentRider,
                        PositionInPull = position,
                        TargetPower = targetPower
                    });
                }

                plan.Pulls.Add(climbPull);
            }
        }

        return plan;
    }

    /// <summary>
    /// Creates a PacelinePlan based on the provided rider power plans and number of rotations.
    /// </summary>
    /// <param name="powerPlans">List of RiderPowerPlan objects representing each rider's power plan.</param>
    /// <param name="rotations">Number of rotations to simulate.</param>
    /// <returns>PacelinePlan object representing the entire race plan.</returns>
    public PacelinePlan CreatePlan(List<RiderPowerPlan> powerPlans, int rotations)
    {
        // Model validation is performed by ParsedModelValidator before composing the plan.
        
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
