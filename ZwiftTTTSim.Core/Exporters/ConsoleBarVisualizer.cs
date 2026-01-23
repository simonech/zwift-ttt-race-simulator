using System.Text;
using ZwiftTTTSim.Core.Model;

namespace ZwiftTTTSim.Core.Exporters;

/// <summary>
/// Generates ASCII bar visualizations for workout power profiles in the console.
/// </summary>
public class ConsoleBarVisualizer
{
    private const int MaxBarHeight = 20;
    private const int MaxRotationsToShow = 2;
    private const int BarWidth = 4; // Width of each vertical bar
    
    // Intensity zone thresholds (matching ImageExporter zones)
    private const double AnaerobicThreshold = 1.18;
    private const double Vo2MaxThreshold = 1.05;
    private const double ThresholdZone = 0.90;
    private const double TempoThreshold = 0.75;
    private const double EnduranceThreshold = 0.60;

    /// <summary>
    /// Creates an ASCII bar visualization for a rider's workout, limited to the first 2 rotations.
    /// </summary>
    /// <param name="riderName">The name of the rider.</param>
    /// <param name="steps">The list of workout steps for the rider.</param>
    /// <param name="totalRiders">The total number of riders in the team.</param>
    /// <param name="totalRotations">The total number of rotations in the workout.</param>
    /// <returns>A string containing the ASCII bar visualization with console color codes.</returns>
    public string CreateBarVisualization(string riderName, List<WorkoutStep> steps, int totalRiders, int totalRotations)
    {
        ArgumentNullException.ThrowIfNull(riderName);
        ArgumentNullException.ThrowIfNull(steps);

        if (steps.Count == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        
        // Calculate how many steps to show (first 2 rotations only)
        var rotationsToShow = Math.Min(totalRotations, MaxRotationsToShow);
        var stepsToShow = Math.Min(steps.Count, rotationsToShow * totalRiders);

        if (stepsToShow == 0)
        {
            return string.Empty;
        }

        // Get the subset of steps to visualize
        var visibleSteps = steps.Take(stepsToShow).ToList();
        
        // Find max power to scale the bars
        var maxPower = visibleSteps.Max(s => s.Power);

        // Header
        sb.AppendLine($"\n  Power Profile (First {rotationsToShow} Rotation{(rotationsToShow > 1 ? "s" : "")}):");
        
        // Draw the vertical bars from top to bottom
        for (int row = MaxBarHeight; row > 0; row--)
        {
            sb.Append("  ");
            
            for (int i = 0; i < visibleSteps.Count; i++)
            {
                var step = visibleSteps[i];
                var barHeight = CalculateBarHeight(step.Power, maxPower);
                var barChar = GetBarCharacter(step.Intensity);
                var color = GetConsoleColor(step.Intensity);
                
                // Determine if this row should show the bar
                if (row <= barHeight)
                {
                    sb.Append($"{{COLOR:{color}}}");
                    sb.Append(new string(barChar, BarWidth));
                    sb.Append("{COLOR:RESET}");
                }
                else
                {
                    sb.Append(new string(' ', BarWidth));
                }
            }
            
            sb.AppendLine();
        }
        
        // Draw baseline
        sb.Append("  ");
        sb.AppendLine(new string('─', visibleSteps.Count * BarWidth));

        return sb.ToString();
    }

    /// <summary>
    /// Calculates the height of a bar based on power relative to max power.
    /// </summary>
    private int CalculateBarHeight(double power, double maxPower)
    {
        if (maxPower == 0)
        {
            return 0;
        }

        var proportionalHeight = (power / maxPower) * MaxBarHeight;
        return Math.Max(1, (int)Math.Round(proportionalHeight));
    }

    /// <summary>
    /// Gets the appropriate bar character based on intensity.
    /// </summary>
    private char GetBarCharacter(double intensity)
    {
        return intensity switch
        {
            >= AnaerobicThreshold => '█', // Anaerobic - solid block
            >= Vo2MaxThreshold => '█',    // VO2 Max - solid block
            >= ThresholdZone => '▓',      // Threshold - dark shade
            >= TempoThreshold => '▒',     // Tempo - medium shade
            >= EnduranceThreshold => '░', // Endurance - light shade
            _ => '·'                      // Recovery - light dot
        };
    }

    /// <summary>
    /// Maps intensity zones to console-safe colors matching the ImageExporter color scheme.
    /// </summary>
    private ConsoleColor GetConsoleColor(double intensity)
    {
        return intensity switch
        {
            >= AnaerobicThreshold => ConsoleColor.Red,        // Anaerobic (Red)
            >= Vo2MaxThreshold => ConsoleColor.DarkYellow,    // VO2 Max (Orange -> DarkYellow)
            >= ThresholdZone => ConsoleColor.Yellow,          // Threshold (Yellow)
            >= TempoThreshold => ConsoleColor.Green,          // Tempo (Green)
            >= EnduranceThreshold => ConsoleColor.Blue,       // Endurance (Blue)
            _ => ConsoleColor.DarkGray                        // Recovery (DarkGray)
        };
    }

    /// <summary>
    /// Renders the bar visualization to the console with proper color handling.
    /// </summary>
    /// <param name="visualization">The visualization string with color codes.</param>
    public void RenderToConsole(string visualization)
    {
        ArgumentNullException.ThrowIfNull(visualization);

        // Split by color markers and render each segment
        var parts = visualization.Split(new[] { "{COLOR:" }, StringSplitOptions.None);
        
        foreach (var part in parts)
        {
            if (part.Contains('}'))
            {
                var colorEnd = part.IndexOf('}');
                var colorName = part.Substring(0, colorEnd);
                var text = part.Substring(colorEnd + 1);

                if (colorName == "RESET")
                {
                    Console.ResetColor();
                }
                else if (Enum.TryParse<ConsoleColor>(colorName, out var color))
                {
                    Console.ForegroundColor = color;
                }

                Console.Write(text);
            }
            else
            {
                Console.Write(part);
            }
        }
    }

    /// <summary>
    /// Generates and renders a legend showing the color mapping for intensity zones.
    /// </summary>
    public void RenderLegend()
    {
        Console.WriteLine("\n  Legend:");
        
        var legendItems = new[]
        {
            (Intensity: "Anaerobic (≥1.18)", Color: ConsoleColor.Red, Char: '█'),
            (Intensity: "VO2 Max (≥1.05)", Color: ConsoleColor.DarkYellow, Char: '█'),
            (Intensity: "Threshold (≥0.90)", Color: ConsoleColor.Yellow, Char: '▓'),
            (Intensity: "Tempo (≥0.75)", Color: ConsoleColor.Green, Char: '▒'),
            (Intensity: "Endurance (≥0.60)", Color: ConsoleColor.Blue, Char: '░'),
            (Intensity: "Recovery (<0.60)", Color: ConsoleColor.DarkGray, Char: '·')
        };

        foreach (var item in legendItems)
        {
            Console.Write("    ");
            Console.ForegroundColor = item.Color;
            Console.Write($"{item.Char}{item.Char}{item.Char}");
            Console.ResetColor();
            Console.WriteLine($" {item.Intensity}");
        }
        
        Console.WriteLine();
    }
}
