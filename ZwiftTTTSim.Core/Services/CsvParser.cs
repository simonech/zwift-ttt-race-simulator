namespace ZwiftTTTSim.Core.Services;

using ZwiftTTTSim.Core.Model;

public class CsvParser
{
    public List<RiderPowerPlan> ParseCsv(string csvContent)
    {
        var riders = new List<RiderPowerPlan>();
        var lines = csvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var line in lines)
        {
            // Skip empty lines or comment lines
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                continue;

            var parts = line.Split(',', StringSplitOptions.TrimEntries);
            if (parts.Length < 7)
                throw new ArgumentException($"Invalid CSV line (expected at least 7 fields): {line}");

            var name = parts[0];
            
            // Parse Weight and FTP (currently not used in workout generation, but part of the rider data model)
            if (!double.TryParse(parts[1], out var weight))
                throw new ArgumentException($"Invalid weight value: {parts[1]}");
            
            if (!double.TryParse(parts[2], out var ftp))
                throw new ArgumentException($"Invalid FTP value: {parts[2]}");
            
            if (!int.TryParse(parts[3], out var pullDurationSeconds))
                throw new ArgumentException($"Invalid PullDuration value: {parts[3]}");

            // Parse power by position (4 values expected)
            var powerByPosition = new int[4];
            for (int i = 0; i < 4; i++)
            {
                if (i + 4 >= parts.Length)
                    throw new ArgumentException($"Missing PowerByPosition[{i}] value in line: {line}");
                
                if (!int.TryParse(parts[i + 4], out powerByPosition[i]))
                    throw new ArgumentException($"Invalid PowerByPosition[{i}] value: {parts[i + 4]}");
            }

            riders.Add(new RiderPowerPlan
            {
                Name = name,
                PullDuration = TimeSpan.FromSeconds(pullDurationSeconds),
                PowerByPosition = powerByPosition,
                Rider = new RiderData 
                { 
                    FTP = ftp, 
                    Weight = weight 
                }
            });
        }

        if (riders.Count == 0)
            throw new ArgumentException("No valid rider data found in CSV content");

        return riders;
    }
}
