using System.CommandLine;
using ZwiftTTTSim.Core.Services;

// Define CLI options
var inputFileOption = new Option<FileInfo>(
    name: "--input",
    description: "Path to the CSV file containing rider data")
{
    IsRequired = true
};
inputFileOption.AddAlias("-i");

var outputFolderOption = new Option<string>(
    name: "--output",
    description: "Output folder for workout files",
    getDefaultValue: () => "workouts");
outputFolderOption.AddAlias("-o");

var rotationsOption = new Option<int>(
    name: "--rotations",
    description: "Number of rotations for the workout",
    getDefaultValue: () => 5);
rotationsOption.AddAlias("-r");

var rootCommand = new RootCommand("Zwift TTT Race Simulator - Generate team time trial workouts")
{
    inputFileOption,
    outputFolderOption,
    rotationsOption
};

rootCommand.SetHandler((inputFile, outputFolder, rotations) =>
{
    try
    {
        // Validate input file exists
        if (!inputFile.Exists)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: Input file '{inputFile.FullName}' not found.");
            Console.ResetColor();
            Environment.Exit(1);
        }

        // Read and parse CSV file
        var csvContent = File.ReadAllText(inputFile.FullName);
        var parser = new CsvParser();
        var powerPlans = parser.ParseCsv(csvContent);

        // Generate workouts
        var workoutCreator = new WorkoutCreator();
        var workouts = workoutCreator.CreateWorkouts(powerPlans, rotations);

        // Print workouts rider by rider
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n╔══════════════════════════════════════╗");
        Console.WriteLine("║  Zwift TTT Race Simulator           ║");
        Console.WriteLine("╚══════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine($"Input File: {inputFile.Name}");
        Console.WriteLine($"Team Size: {powerPlans.Count} riders");
        Console.WriteLine($"Rotations: {rotations}");
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
        exporter.ExportToFiles(workouts, outputFolder);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n✅ Workouts exported to '{outputFolder}' directory");
        Console.ResetColor();
        Console.WriteLine($"   Generated {workouts.Count} ZWO files:");
        foreach (var riderName in workouts.Keys)
        {
            var fileName = ZwoExporter.GetWorkoutFileName(riderName);
            Console.WriteLine($"   - {fileName}");
        }

        // Export workout images
        var imageExporter = new ImageExporter();
        imageExporter.ExportToFiles(workouts, outputFolder, powerPlans.Count, powerPlans);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n✅ Workout images exported to '{outputFolder}' directory");
        Console.ResetColor();
        Console.WriteLine($"   Generated {workouts.Count} PNG files:");
        foreach (var riderName in workouts.Keys)
        {
            var fileName = ImageExporter.GetWorkoutImageFileName(riderName);
            Console.WriteLine($"   - {fileName}");
        }
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error: {ex.Message}");
        Console.ResetColor();
        Environment.Exit(1);
    }
}, inputFileOption, outputFolderOption, rotationsOption);

return await rootCommand.InvokeAsync(args);

