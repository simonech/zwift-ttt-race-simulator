using System.CommandLine;
using ZwiftTTTSim.Core.Model;
using ZwiftTTTSim.Core.Services;

// NOTE:
// This CLI uses System.CommandLine 2.0.0-beta4.22272.1.
// Later 2.x versions are API-incompatible.

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
        var rotationComposer = new RotationComposer();
        var pulls = rotationComposer.CreatePullsList(powerPlans, rotations);


        // Print workouts rider by rider
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n╔══════════════════════════════════════╗");
        Console.WriteLine("║  Zwift TTT Race Simulator           ║");
        Console.WriteLine("╚══════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine($"Input File: {inputFile.Name}");
        Console.WriteLine($"Team Size: {powerPlans.Count} riders");
        Console.WriteLine($"Rotations: {rotations}");
        Console.WriteLine($"Total Steps per Rider: {pulls.Count / powerPlans.Count}");
        Console.WriteLine();


        foreach (var pull in pulls)
        {
            Console.WriteLine($"Pull {pull.PullNumber}: Duration {pull.PullDuration.TotalSeconds} seconds");
            foreach (var position in pull.PullPositions)
            {
                Console.WriteLine($"  Position {position.PositionInPull + 1}: Rider {position.Rider.Name}, Target Power {position.TargetPower} W");
            }
        }

        var sampleWorkout = new List<WorkoutStep>()
        {
            new WorkoutStep
            {
                DurationSeconds = 300,
                Power = 100,
                Intensity = 0.5
            },
            new WorkoutStep
            {
                DurationSeconds = 600,
                Power = 200,
                Intensity = 0.75
            },
            new WorkoutStep
            {
                DurationSeconds = 300,
                Power = 150,
                Intensity = 0.6
            }
        };

        Dictionary<string, List<WorkoutStep>> workouts = new Dictionary<string, List<WorkoutStep>>()
        {
            { "Rider 1", sampleWorkout },
            { "Rider 2", sampleWorkout },
            { "Rider 3", sampleWorkout }
        };

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

