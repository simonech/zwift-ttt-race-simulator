using System.Globalization;
using SkiaSharp;
using ZwiftTTTSim.Core.Model;

namespace ZwiftTTTSim.Core.Services;

public class ImageExporter
{
    private const int ChartWidth = 1200;
    private const int ChartHeight = 600;
    private const int Padding = 60;
    private const int AxisPadding = 40;
    private const double PowerRangePaddingMultiplier = 1.1;
    private const int PowerAxisSteps = 5;
    private const int TimeAxisSteps = 10;

    private static readonly HashSet<char> InvalidFileNameChars = new(Path.GetInvalidFileNameChars());

    public static string GetWorkoutImageFileName(string riderName)
    {
        ArgumentNullException.ThrowIfNull(riderName);
        
        // Sanitize the rider name to create a valid filename
        var sanitizedName = string.Concat(riderName.Select(c => InvalidFileNameChars.Contains(c) ? '_' : c));
        return $"{sanitizedName}_TTT_Workout.png";
    }

    public byte[] ExportToImage(string riderName, List<WorkoutStep> steps, int riderIndex, int totalRiders)
    {
        ArgumentNullException.ThrowIfNull(riderName);
        ArgumentNullException.ThrowIfNull(steps);
        
        if (steps.Count == 0)
        {
            throw new ArgumentException("Steps list cannot be empty", nameof(steps));
        }

        using var surface = SKSurface.Create(new SKImageInfo(ChartWidth, ChartHeight));
        var canvas = surface.Canvas;
        
        // Clear background to white
        canvas.Clear(SKColors.White);

        // Calculate chart area
        var chartLeft = Padding + AxisPadding;
        var chartTop = Padding;
        var chartRight = ChartWidth - Padding;
        var chartBottom = ChartHeight - Padding - AxisPadding;
        var chartAreaWidth = chartRight - chartLeft;
        var chartAreaHeight = chartBottom - chartTop;

        // Find max power and total duration
        var maxPower = steps.Max(s => s.Power);
        var totalDuration = steps.Sum(s => s.DurationSeconds);

        // Add some padding to max power for better visualization
        var powerRange = maxPower * PowerRangePaddingMultiplier;

        // Draw title
        using var titleFont = new SKFont(SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), 24);
        using var titlePaint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true
        };
        var titleText = $"TTT Workout - {riderName}";
        var titleWidth = titleFont.MeasureText(titleText);
        canvas.DrawText(titleText, (ChartWidth - titleWidth) / 2, 30, titleFont, titlePaint);

        // Draw axes
        using var axisPaint = new SKPaint
        {
            Color = SKColors.Black,
            StrokeWidth = 2,
            IsAntialias = true
        };
        
        // Y-axis
        canvas.DrawLine(chartLeft, chartTop, chartLeft, chartBottom, axisPaint);
        // X-axis
        canvas.DrawLine(chartLeft, chartBottom, chartRight, chartBottom, axisPaint);

        // Draw axis labels
        using var labelFont = new SKFont(SKTypeface.FromFamilyName("Arial"), 12);
        using var labelPaint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true
        };

        // Y-axis label (Power)
        using var yLabelFont = new SKFont(SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), 14);
        using var yLabelPaint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true
        };
        canvas.Save();
        canvas.RotateDegrees(-90, 20, ChartHeight / 2);
        canvas.DrawText("Power (W)", 20, ChartHeight / 2, yLabelFont, yLabelPaint);
        canvas.Restore();

        // X-axis label (Time)
        using var xLabelFont = new SKFont(SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), 14);
        using var xLabelPaint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true
        };
        canvas.DrawText("Time (s)", ChartWidth / 2 - 30, ChartHeight - 10, xLabelFont, xLabelPaint);

        // Draw Y-axis ticks and labels (power)
        for (int i = 0; i <= PowerAxisSteps; i++)
        {
            var power = (powerRange / PowerAxisSteps) * i;
            var y = (float)(chartBottom - (chartAreaHeight * i / PowerAxisSteps));
            
            // Tick mark
            canvas.DrawLine(chartLeft - 5, y, chartLeft, y, axisPaint);
            
            // Label
            var powerText = ((int)power).ToString(CultureInfo.InvariantCulture);
            var textWidth = labelFont.MeasureText(powerText);
            var textHeight = labelFont.Size; // Approximate height from font size
            canvas.DrawText(powerText, chartLeft - textWidth - 10, y + textHeight / 2, labelFont, labelPaint);
        }

        // Draw bars
        var currentX = (float)chartLeft;
        
        for (int i = 0; i < steps.Count; i++)
        {
            var step = steps[i];
            
            // Calculate bar dimensions
            var barWidth = (float)((chartAreaWidth * step.DurationSeconds) / totalDuration);
            var barHeight = (float)((chartAreaHeight * step.Power) / powerRange);
            
            // Determine bar color based on position
            var barColor = step.Intensity switch
            {
                >= 1.18 => new SKColor(220, 50, 50),      // Red for Anaerobic
                >= 1.05 => new SKColor(255, 165, 0),      // Orange for VO2 Max
                >= 0.90 => new SKColor(255, 200, 0),      // Yellow for Threshold
                >= 0.75 => new SKColor(50, 180, 50),      // Green for Tempo
                >= 0.60 => new SKColor(0, 0, 255),        // Blue for Endurance
                _ => new SKColor(105, 105, 105)           // DarkGray for Recovery
            };

            // Draw bar
            using var barPaint = new SKPaint
            {
                Color = barColor,
                IsAntialias = true
            };
            
            var barRect = new SKRect(
                currentX,
                chartBottom - barHeight,
                currentX + barWidth,
                chartBottom
            );
            canvas.DrawRect(barRect, barPaint);

            // Draw bar outline
            using var outlinePaint = new SKPaint
            {
                Color = SKColors.Black,
                StrokeWidth = 1,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };
            canvas.DrawRect(barRect, outlinePaint);

            currentX += barWidth;
        }

        // Draw X-axis ticks and labels (time)
        for (int i = 0; i <= TimeAxisSteps; i++)
        {
            var time = (totalDuration / TimeAxisSteps) * i;
            var x = chartLeft + (chartAreaWidth * i / TimeAxisSteps);
            
            // Tick mark
            canvas.DrawLine(x, chartBottom, x, chartBottom + 5, axisPaint);
            
            // Label
            var minutes = time / 60;
            var seconds = time % 60;
            
            var timeText = $"{minutes:D2}:{seconds:D2}";
            var textWidth = labelFont.MeasureText(timeText);
            canvas.DrawText(timeText, x - textWidth / 2, chartBottom + 20, labelFont, labelPaint);
        }

        // Draw legend
        var legendX = chartRight - 120;
        var legendY = chartTop + 20;
        var legendBoxSize = 15;
        var legendSpacing = 25;

        using var legendTextFont = new SKFont(SKTypeface.FromFamilyName("Arial"), 12);
        using var legendTextPaint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true
        };

        var legendItems = new[]
        {
            (Color: new SKColor(220, 50, 50), Text: "Anaerobic (>= 1.18)"),
            (Color: new SKColor(255, 165, 0), Text: "VO2 Max (>= 1.05)"),
            (Color: new SKColor(255, 200, 0), Text: "Threshold (>= 0.90)"),
            (Color: new SKColor(50, 180, 50), Text: "Tempo (>= 0.75)"),
            (Color: new SKColor(0, 0, 255), Text: "Endurance (>= 0.60)"),
            (Color: new SKColor(105, 105, 105), Text: "Recovery (< 0.60)")
        };

        for (int i = 0; i < legendItems.Length; i++)
        {
            var item = legendItems[i];
            var y = legendY + (i * legendSpacing);
            
            // Draw color box
            using var legendBoxPaint = new SKPaint
            {
                Color = item.Color,
                IsAntialias = true
            };
            canvas.DrawRect(legendX, y, legendBoxSize, legendBoxSize, legendBoxPaint);
            
            // Draw box outline
            using var legendOutlinePaint = new SKPaint
            {
                Color = SKColors.Black,
                StrokeWidth = 1,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };
            canvas.DrawRect(legendX, y, legendBoxSize, legendBoxSize, legendOutlinePaint);
            
            // Draw text
            canvas.DrawText(item.Text, legendX + legendBoxSize + 5, y + legendBoxSize - 2, legendTextFont, legendTextPaint);
        }

        // Encode to PNG
        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    public void ExportToFiles(Dictionary<string, List<WorkoutStep>> workouts, string outputDirectory, int totalRiders)
    {
        ArgumentNullException.ThrowIfNull(workouts);
        ArgumentNullException.ThrowIfNull(outputDirectory);
        
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        var riderIndex = 0;
        foreach (var (riderName, steps) in workouts)
        {
            var imageData = ExportToImage(riderName, steps, riderIndex, totalRiders);
            var fileName = GetWorkoutImageFileName(riderName);
            var filePath = Path.Combine(outputDirectory, fileName);
            File.WriteAllBytes(filePath, imageData);
            riderIndex++;
        }
    }
}
