using System.Collections.Generic;
using ZwiftTTTSim.Core.Model.Segments;

namespace ZwiftTTTSim.Core.Model.Config;

public class RaceConfig
{
    public string Name { get; set; } = string.Empty;

    public List<ISegment> Route { get; set; } = new List<ISegment>();
}
