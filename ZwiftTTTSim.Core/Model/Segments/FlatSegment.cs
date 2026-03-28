using System;

namespace ZwiftTTTSim.Core.Model.Segments;

/// <summary>
/// Represents a flat segment where duration is dynamically estimated 
/// based on distance and average speed expectations.
/// </summary>
public class FlatSegment : ISegment
{
    /// <summary>
    /// Gets or sets the distance of the segment in kilometers.
    /// </summary>
    public double DistanceKm { get; set; }

    /// <summary>
    /// Gets or sets the target average speed for the segment in kilometers per hour.
    /// </summary>
    public double AvgSpeedKph { get; set; }

    /// <summary>
    /// Gets the calculated duration of the segment.
    /// Ensures duration consistency for JSON serialization and core logic.
    /// </summary>
    public TimeSpan Duration
    {
        get
        {
            if (AvgSpeedKph <= 0)
            {
                return TimeSpan.Zero;
            }

            return TimeSpan.FromSeconds((DistanceKm / AvgSpeedKph) * 3600);
        }
    }
}
