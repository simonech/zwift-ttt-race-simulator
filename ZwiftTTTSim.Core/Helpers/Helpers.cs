namespace ZwiftTTTSim.Core.Helpers;
public static class Helpers
{
    /// <summary>
    /// Calculates the rider's position in the rotation based on their index, the index of the rider currently pulling, and total riders.
    /// </summary>
    /// <example>
    /// For 5 riders (0 to 4):
    /// - If we are considering the first rider (riderIndex = 0) and the first rider is pulling (pullingRiderIndex = 0), its position is 0 (first position).
    /// - If the second rider (riderIndex = 1) is considered while the first rider is pulling (pullingRiderIndex = 0), its position is 1 (second position).
    /// - If the first rider (riderIndex = 0) is considered while the second rider is pulling (pullingRiderIndex = 1), its position is 4 (last position).
    /// </example>
    /// <param name="riderIndex">The initial index of the rider in the rotation (0 to totalRiders - 1).</param>
    /// <param name="pullingRiderIndex">Index of the rider currently pulling.</param>
    /// <param name="totalRiders">The total number of riders in the rotation.</param>
    /// <returns>The position of the rider in the rotation.</returns>
    public static int GetRotationPosition(int riderIndex, int pullingRiderIndex, int totalRiders)
    {
        return (riderIndex - pullingRiderIndex + totalRiders) % totalRiders;
    }
}