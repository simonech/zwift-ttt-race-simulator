namespace ZwiftTTTSim.Core.Services;

using ZwiftTTTSim.Core.Exceptions;
using ZwiftTTTSim.Core.Model;

public class CsvParser
{
    public List<RiderPowerPlan> ParseCsv(string csvContent)
    {
        var riders = new List<RiderPowerPlan>();
        var lines = csvContent.Split('\n');

        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            var lineNumber = lineIndex + 1;
            var line = lines[lineIndex].Trim();

            // Skip empty lines or comment lines
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                continue;

            var parts = line.Split(',', StringSplitOptions.TrimEntries);
            if (parts.Length != 8)
                throw new CsvParseException(
                    lineNumber: lineNumber,
                    lineContent: line,
                    message: "Expected exactly 8 fields:\n Name, Weight, FTP, Pull Duration, Pull Power, 2nd, 3rd, draft");

            var name = parts[0];
            
            // Parse Weight and FTP (currently not used in workout generation, but part of the rider data model)
            if (!double.TryParse(parts[1], out var weight))
                throw new CsvParseException(lineNumber: lineNumber, lineContent: line, message: $"Invalid weight value: {parts[1]}");
            
            if (!double.TryParse(parts[2], out var ftp))
                throw new CsvParseException(lineNumber: lineNumber, lineContent: line, message: $"Invalid FTP value: {parts[2]}");
            
            if (!int.TryParse(parts[3], out var pullDurationSeconds))
                throw new CsvParseException(lineNumber: lineNumber, lineContent: line, message: $"Invalid PullDuration value: {parts[3]}");

            // Parse power by position (4 values expected)
            var powerByPosition = new int[4];
            for (int i = 0; i < 4; i++)
            {
                if (i + 4 >= parts.Length)
                    throw new CsvParseException(lineNumber: lineNumber, lineContent: line, message: $"Missing PowerByPosition[{i}] value");

                
                if (!int.TryParse(parts[i + 4], out powerByPosition[i]))
                    throw new CsvParseException(lineNumber: lineNumber, lineContent: line, message: $"Invalid PowerByPosition[{i}] value: {parts[i + 4]}");
            }

            riders.Add(new RiderPowerPlan
            {
                Name = name,
                PullDuration = TimeSpan.FromSeconds(pullDurationSeconds),
                PowerByPosition = powerByPosition,
                RiderData = new RiderData 
                { 
                    FTP = ftp, 
                    Weight = weight 
                }
            });
        }

        if (riders.Count == 0)
            throw new CsvParseException(lineNumber: 0, lineContent: "", message: "No valid rider data found in CSV content");

        return riders;
    }
}
