using System.Globalization;
using SkiaSharp;
using ZwiftTTTSim.Core.Model;

namespace ZwiftTTTSim.Core.Services;

public class ImageExporter
{
    private const int ChartWidth = 1200;
    private const int ChartHeight = 650;
    private const int Padding = 60;
    private const int AxisPadding = 40;
    private const double PowerRangePaddingMultiplier = 1.1;
    private const int PowerAxisSteps = 5;
    private const int TimeAxisSteps = 10;
    private const int XAxisLabelTopMargin = 35;
    private const int LegendTopMargin = 45;
    private const int BarSpacing = 2; // Spacing between bars in pixels
    private const float FtpDashLength = 4; // Length of dash segments in FTP line
    private const float FtpGapLength = 4; // Length of gap segments in FTP line

    private static readonly HashSet<char> InvalidFileNameChars = new(Path.GetInvalidFileNameChars());

    public static string GetWorkoutImageFileName(string riderName)
    {
        ArgumentNullException.ThrowIfNull(riderName);
        
        // Sanitize the rider name to create a valid filename
        var sanitizedName = string.Concat(riderName.Select(c => InvalidFileNameChars.Contains(c) ? '_' : c));
        return $"{sanitizedName}_TTT_Workout.png";
    }

    public byte[] ExportToImage(string riderName, List<WorkoutStep> steps, int riderIndex, int totalRiders, double ftp = 0)
    {
        ArgumentNullException.ThrowIfNull(riderName);
        ArgumentNullException.ThrowIfNull(steps);
        
        if (steps.Count == 0)
        {
            throw new ArgumentException("Steps list cannot be empty", nameof(steps));
        }

        using var surface = SKSurface.Create(new SKImageInfo(ChartWidth, ChartHeight));
        var canvas = surface.Canvas;
        
        // Clear background to dark navy blue (matching banner design)
        canvas.Clear(new SKColor(26, 35, 50)); // #1a2332

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
            Color = SKColors.White, // White text for dark background
            IsAntialias = true
        };
        var titleText = $"TTT Workout - {riderName}";
        var titleWidth = titleFont.MeasureText(titleText);
        canvas.DrawText(titleText, (ChartWidth - titleWidth) / 2, 30, titleFont, titlePaint);

        // Draw axes
        using var axisPaint = new SKPaint
        {
            Color = new SKColor(150, 150, 150), // Light gray for dark background
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
            Color = new SKColor(200, 200, 200), // Light gray for labels
            IsAntialias = true
        };

        // Y-axis label (Power)
        using var yLabelFont = new SKFont(SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), 14);
        using var yLabelPaint = new SKPaint
        {
            Color = SKColors.White, // White for axis label
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
            Color = SKColors.White, // White for axis label
            IsAntialias = true
        };
        var xLabelText = "Time (s)";
        var xLabelWidth = xLabelFont.MeasureText(xLabelText);
        canvas.DrawText(xLabelText, (ChartWidth - xLabelWidth) / 2, chartBottom + XAxisLabelTopMargin, xLabelFont, xLabelPaint);

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
            
            // Calculate bar dimensions with spacing
            var barWidth = (float)((chartAreaWidth * step.DurationSeconds) / totalDuration);
            var actualBarWidth = Math.Max(1, barWidth - BarSpacing); // Ensure minimum width of 1 pixel
            var barHeight = (float)((chartAreaHeight * step.Power) / powerRange);
            
            // Determine bar color based on intensity zone - matching banner design
            var barColor = step.Intensity switch
            {
                >= 1.18 => new SKColor(231, 76, 60),      // Red #e74c3c for Anaerobic
                >= 1.05 => new SKColor(244, 125, 66),     // Orange #f47d42 for VO2 Max
                >= 0.90 => new SKColor(244, 197, 66),     // Yellow #f4c542 for Threshold
                >= 0.75 => new SKColor(90, 181, 94),      // Green #5ab55e for Tempo
                >= 0.60 => new SKColor(74, 155, 155),     // Teal #4a9b9b for Endurance
                _ => new SKColor(59, 90, 125)             // Blue #3b5a7d for Recovery
            };

            // Draw bar with gradient (darker at bottom, lighter at top)
            var barRect = new SKRect(
                currentX,
                chartBottom - barHeight,
                currentX + actualBarWidth,
                chartBottom
            );
            
            // Create gradient from darker (bottom) to lighter (top)
            var darkerColor = new SKColor(
                (byte)(barColor.Red * 0.7),
                (byte)(barColor.Green * 0.7),
                (byte)(barColor.Blue * 0.7)
            );
            var lighterColor = new SKColor(
                (byte)Math.Min(255, barColor.Red * 1.2),
                (byte)Math.Min(255, barColor.Green * 1.2),
                (byte)Math.Min(255, barColor.Blue * 1.2)
            );
            
            using var gradient = SKShader.CreateLinearGradient(
                new SKPoint(currentX, chartBottom),           // Start at bottom
                new SKPoint(currentX, chartBottom - barHeight), // End at top
                new[] { darkerColor, lighterColor },
                new[] { 0f, 1f },
                SKShaderTileMode.Clamp
            );
            
            using var barPaint = new SKPaint
            {
                Shader = gradient,
                IsAntialias = true
            };
            
            canvas.DrawRect(barRect, barPaint);

            // Draw bar outline (subtle dark outline for definition)
            using var outlinePaint = new SKPaint
            {
                Color = new SKColor(20, 20, 20, 100), // Dark semi-transparent outline
                StrokeWidth = 1,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };
            canvas.DrawRect(barRect, outlinePaint);

            currentX += barWidth; // Move full width including spacing
        }

        // Draw FTP horizontal line if FTP is provided and within visible range
        if (ftp > 0 && ftp <= powerRange)
        {
            var ftpY = (float)(chartBottom - (chartAreaHeight * ftp / powerRange));
            
            // Draw white dotted line at FTP (matching banner design)
            using var ftpLinePaint = new SKPaint
            {
                Color = SKColors.White, // White dotted line
                StrokeWidth = 2,
                IsAntialias = true,
                PathEffect = SKPathEffect.CreateDash(new float[] { FtpDashLength, FtpGapLength }, 0)
            };
            canvas.DrawLine(chartLeft, ftpY, chartRight, ftpY, ftpLinePaint);
            
            // Draw FTP label in white
            using var ftpLabelFont = new SKFont(SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), 12);
            using var ftpLabelPaint = new SKPaint
            {
                Color = SKColors.White, // White text
                IsAntialias = true
            };
            var ftpText = $"FTP: {Math.Round(ftp):F0}W";
            var ftpTextWidth = ftpLabelFont.MeasureText(ftpText);
            canvas.DrawText(ftpText, chartRight - ftpTextWidth - 10, ftpY - 5, ftpLabelFont, ftpLabelPaint);
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

        // Draw legend at the bottom
        var legendBoxSize = 15;
        var legendSpacing = 25;
        var legendItemsPerRow = 3;
        var legendItems = new[]
        {
            (Color: new SKColor(231, 76, 60), Text: "Anaerobic (>= 1.18)"),      // #e74c3c
            (Color: new SKColor(244, 125, 66), Text: "VO2 Max (>= 1.05)"),      // #f47d42
            (Color: new SKColor(244, 197, 66), Text: "Threshold (>= 0.90)"),    // #f4c542
            (Color: new SKColor(90, 181, 94), Text: "Tempo (>= 0.75)"),         // #5ab55e
            (Color: new SKColor(74, 155, 155), Text: "Endurance (>= 0.60)"),    // #4a9b9b
            (Color: new SKColor(59, 90, 125), Text: "Recovery (< 0.60)")        // #3b5a7d
        };
        
        // Calculate legend width for centering
        var legendColumnWidth = 200;
        var legendTotalWidth = legendItemsPerRow * legendColumnWidth;
        var legendStartX = (ChartWidth - legendTotalWidth) / 2;
        var legendStartY = chartBottom + LegendTopMargin;

        using var legendTextFont = new SKFont(SKTypeface.FromFamilyName("Arial"), 12);
        using var legendTextPaint = new SKPaint
        {
            Color = new SKColor(200, 200, 200), // Light gray text for dark background
            IsAntialias = true
        };

        for (int i = 0; i < legendItems.Length; i++)
        {
            var item = legendItems[i];
            var row = i / legendItemsPerRow;
            var col = i % legendItemsPerRow;
            var legendX = legendStartX + (col * legendColumnWidth);
            var y = legendStartY + (row * legendSpacing);
            
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
                Color = new SKColor(100, 100, 100), // Lighter outline for dark background
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

    public void ExportToFiles(Dictionary<string, List<WorkoutStep>> workouts, string outputDirectory, int totalRiders, List<RiderPowerPlan>? powerPlans = null)
    {
        ArgumentNullException.ThrowIfNull(workouts);
        ArgumentNullException.ThrowIfNull(outputDirectory);
        
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        // Create dictionary lookup for better performance
        var ftpLookup = powerPlans?.ToDictionary(p => p.Name, p => p.Rider.FTP) ?? new Dictionary<string, double>();

        var riderIndex = 0;
        foreach (var (riderName, steps) in workouts)
        {
            var ftp = ftpLookup.TryGetValue(riderName, out var ftpValue) ? ftpValue : 0;
            var imageData = ExportToImage(riderName, steps, riderIndex, totalRiders, ftp);
            var fileName = GetWorkoutImageFileName(riderName);
            var filePath = Path.Combine(outputDirectory, fileName);
            File.WriteAllBytes(filePath, imageData);
            riderIndex++;
        }
    }
}
