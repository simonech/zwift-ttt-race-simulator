using System;

namespace ZwiftTTTSim.Core.Model.Segments;

/// <summary>
/// Represents a climbing segment with a predefined fixed duration.
/// </summary>
public class ClimbSegment : ISegment
{
    /// <summary>
    /// Gets or sets the fixed duration of the climb in seconds.
    /// </summary>
    public double DurationSeconds { get; set; }

    /// <summary>
    /// Gets the time representation of the segment's duration limit.
    /// Ensures duration consistency for JSON serialization and core logic.
    /// </summary>
    public TimeSpan Duration => TimeSpan.FromSeconds(DurationSeconds);
}
