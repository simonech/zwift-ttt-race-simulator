using System;

namespace ZwiftTTTSim.Core.Model.Segments;

/// <summary>
/// Defines a race segment with a calculable or predefined duration.
/// </summary>
public interface ISegment
{
    /// <summary>
    /// Gets the computed or defined duration for the segment workflow.
    /// </summary>
    TimeSpan Duration { get; }
}
