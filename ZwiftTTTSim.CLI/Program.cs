using ZwiftTTTSim.Core.Model;
using ZwiftTTTSim.Core.Services;

// Initialize 6 riders with power plans
var powerPlans = new List<RiderPowerPlan>
{
    new RiderPowerPlan
    {
        Name = "Alice",
        PullDuration = TimeSpan.FromSeconds(30),
        PowerByPosition = [370, 320, 280, 150],
        Rider = new RiderData { FTP = 300, Weight = 75 }
    },
    new RiderPowerPlan
    {
        Name = "Bob",
        PullDuration = TimeSpan.FromSeconds(45),
        PowerByPosition = [330, 290, 270, 240],
        Rider = new RiderData { FTP = 280, Weight = 70 }
    },
    new RiderPowerPlan
    {
        Name = "Charlie",
        PullDuration = TimeSpan.FromSeconds(60),
        PowerByPosition = [370, 320, 300, 270],
        Rider = new RiderData { FTP = 280, Weight = 80 }
    },
    new RiderPowerPlan
    {
        Name = "Diana",
        PullDuration = TimeSpan.FromSeconds(90),
        PowerByPosition = [340, 310, 290, 260],
        Rider = new RiderData { FTP = 290, Weight = 65 }
    },
    new RiderPowerPlan
    {
        Name = "Simone",
        PullDuration = TimeSpan.FromSeconds(45),
        PowerByPosition = [280, 260, 230, 200],
        Rider = new RiderData { FTP = 260, Weight = 60 }
    },
    new RiderPowerPlan
    {
        Name = "Eve",
        PullDuration = TimeSpan.FromSeconds(40),
        PowerByPosition = [310, 280, 250, 220],
        Rider = new RiderData { FTP = 270, Weight = 68 }
    }
};

// Generate workouts with 5 rotations
var workoutCreator = new WorkoutCreator();
var workouts = workoutCreator.CreateWorkouts(powerPlans, rotations: 5);

// Print workouts rider by rider
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("\n╔══════════════════════════════════════╗");
Console.WriteLine("║  Zwift TTT Race Simulator           ║");
Console.WriteLine("╚══════════════════════════════════════╝");
Console.ResetColor();
Console.WriteLine($"Team Size: {powerPlans.Count} riders");
Console.WriteLine($"Rotations: 5");
Console.WriteLine($"Total Steps per Rider: {workouts.First().Value.Count}");
Console.WriteLine();

int riderIndex = 0;
var riderColors = new[] { ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Magenta, ConsoleColor.Blue, ConsoleColor.Red, ConsoleColor.Cyan };

foreach (var (riderName, steps) in workouts)
{
    Console.ForegroundColor = riderColors[riderIndex % riderColors.Length];
    Console.WriteLine($"\n═══ Workout for {riderName} ═══");
    Console.ResetColor();
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine($"{"Step",-6} {"Position",-10} {"Duration (s)",-14} {"Power (W)",-12}");
    Console.WriteLine(new string('─', 45));
    Console.ResetColor();

    for (int i = 0; i < steps.Count; i++)
    {
        var pullingRiderIndex = i % powerPlans.Count;
        var position = (riderIndex - pullingRiderIndex + powerPlans.Count) % powerPlans.Count + 1;

        // Color position based on effort (1=pulling hard, 2-3=moderate, 4+=easy)
        Console.Write($"{i + 1,-6} ");
        Console.ForegroundColor = position switch
        {
            1 => ConsoleColor.Red,
            2 or 3 => ConsoleColor.Yellow,
            _ => ConsoleColor.Green
        };
        Console.Write($"{position,-10}");
        Console.ResetColor();
        Console.Write($" {steps[i].DurationSeconds,-14}");

        // Color power based on intensity
        var powerColor = steps[i].Intensity switch
        {
            >= 1.18 => ConsoleColor.Red,
            >= 1.05 => ConsoleColor.Magenta,
            >= 0.90 => ConsoleColor.Yellow,
            >= 0.75 => ConsoleColor.Green,
            >= 0.60 => ConsoleColor.Blue,
            _ => ConsoleColor.DarkGray
        };
        Console.ForegroundColor = powerColor;
        Console.WriteLine($" {steps[i].Power,-12}");
        Console.ResetColor();
    }

    riderIndex++;
}

Console.WriteLine();

// Export workouts to ZWO files
var exporter = new ZwoExporter();
var outputDirectory = "workouts";
exporter.ExportToFiles(workouts, outputDirectory);

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"\n✅ Workouts exported to '{outputDirectory}' directory");
Console.ResetColor();
Console.WriteLine($"   Generated {workouts.Count} ZWO files:");
foreach (var riderName in workouts.Keys)
{
    var fileName = ZwoExporter.GetWorkoutFileName(riderName);
    Console.WriteLine($"   - {fileName}");
}

// Export workout images
var imageExporter = new ImageExporter();
imageExporter.ExportToFiles(workouts, outputDirectory, powerPlans.Count);

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"\n✅ Workout images exported to '{outputDirectory}' directory");
Console.ResetColor();
Console.WriteLine($"   Generated {workouts.Count} PNG files:");
foreach (var riderName in workouts.Keys)
{
    var fileName = ImageExporter.GetWorkoutImageFileName(riderName);
    Console.WriteLine($"   - {fileName}");
}
Console.WriteLine();

