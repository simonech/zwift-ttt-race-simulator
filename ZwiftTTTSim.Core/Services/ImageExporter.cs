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
        using var titlePaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 24,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
        };
        var titleText = $"TTT Workout - {riderName}";
        var titleBounds = new SKRect();
        titlePaint.MeasureText(titleText, ref titleBounds);
        canvas.DrawText(titleText, (ChartWidth - titleBounds.Width) / 2, 30, titlePaint);

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
        using var labelPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 12,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial")
        };

        // Y-axis label (Power)
        using var yLabelPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 14,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
        };
        canvas.Save();
        canvas.RotateDegrees(-90, 20, ChartHeight / 2);
        canvas.DrawText("Power (W)", 20, ChartHeight / 2, yLabelPaint);
        canvas.Restore();

        // X-axis label (Time)
        using var xLabelPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 14,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
        };
        canvas.DrawText("Time (s)", ChartWidth / 2 - 30, ChartHeight - 10, xLabelPaint);

        // Draw Y-axis ticks and labels (power)
        for (int i = 0; i <= PowerAxisSteps; i++)
        {
            var power = (powerRange / PowerAxisSteps) * i;
            var y = (float)(chartBottom - (chartAreaHeight * i / PowerAxisSteps));
            
            // Tick mark
            canvas.DrawLine(chartLeft - 5, y, chartLeft, y, axisPaint);
            
            // Label
            var powerText = ((int)power).ToString(CultureInfo.InvariantCulture);
            var textBounds = new SKRect();
            labelPaint.MeasureText(powerText, ref textBounds);
            canvas.DrawText(powerText, chartLeft - textBounds.Width - 10, y + textBounds.Height / 2, labelPaint);
        }

        // Draw bars
        var currentX = (float)chartLeft;
        
        for (int i = 0; i < steps.Count; i++)
        {
            var step = steps[i];
            
            // Calculate bar dimensions
            var barWidth = (float)((chartAreaWidth * step.DurationSeconds) / totalDuration);
            var barHeight = (float)((chartAreaHeight * step.Power) / powerRange);
            
            // Calculate position in rotation (1st, 2nd, 3rd, 4th+)
            var pullingRiderIndex = i % totalRiders;
            var position = (riderIndex - pullingRiderIndex + totalRiders) % totalRiders + 1;
            
            // Determine bar color based on position
            var barColor = position switch
            {
                1 => new SKColor(220, 50, 50),      // Red for 1st position
                2 or 3 => new SKColor(255, 200, 0), // Yellow for 2nd and 3rd
                _ => new SKColor(50, 180, 50)       // Green for 4th+
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
            var timeText = ((int)time).ToString(CultureInfo.InvariantCulture);
            var textBounds = new SKRect();
            labelPaint.MeasureText(timeText, ref textBounds);
            canvas.DrawText(timeText, x - textBounds.Width / 2, chartBottom + 20, labelPaint);
        }

        // Draw legend
        var legendX = chartRight - 120;
        var legendY = chartTop + 20;
        var legendBoxSize = 15;
        var legendSpacing = 25;

        using var legendTextPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 12,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Arial")
        };

        var legendItems = new[]
        {
            (Color: new SKColor(220, 50, 50), Text: "1st Position"),
            (Color: new SKColor(255, 200, 0), Text: "2nd-3rd Position"),
            (Color: new SKColor(50, 180, 50), Text: "4th+ Position")
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
            canvas.DrawText(item.Text, legendX + legendBoxSize + 5, y + legendBoxSize - 2, legendTextPaint);
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
