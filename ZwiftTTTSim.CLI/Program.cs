using System.CommandLine;
using ZwiftTTTSim.Core.Model;
using ZwiftTTTSim.Core.Services;
using ZwiftTTTSim.Core.Exporters;
using System.Reflection;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;

// NOTE:
// This CLI uses System.CommandLine 2.0.0-beta4.22272.1.
// Later 2.x versions are API-incompatible.

// Root command
var rootCommand = new RootCommand("Zwift TTT Race Simulator - Generate team time trial workouts");

// Define CLI options

var inputFileOption = new Option<FileInfo?>(
    name: "--input",
    description: "Path to the CSV file containing rider power plans and data",
    parseArgument: result =>
    {
        var filePath = result.Tokens.Count > 0 ? result.Tokens[0].Value : null;
        if (filePath == null)
        {
            result.ErrorMessage = "Input file path is required.";
            return null;
        }

        var fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists)
        {
            result.ErrorMessage = $"Input file '{fileInfo.FullName}' not found.";
            return null;
        }

        return fileInfo;
    })
    {  IsRequired = true };
inputFileOption.AddAlias("-i");

var rotationsOption = new Option<int>(
    name: "--rotations",
    description: "Number of rotations to simulate (default: 5)",
    getDefaultValue: () => 5);
rotationsOption.AddAlias("-r");

var outputFolderOption = new Option<DirectoryInfo?>(
    name: "--output",
    description: "Output folder for generated workout files (default: workouts)",
    getDefaultValue: () => new DirectoryInfo("workouts"));
outputFolderOption.AddAlias("-o");

var formatsOption = new Option<string[]>(
    name: "--format",
    description: "Output file format: zwo, image",
    parseArgument: result =>
    {
        var validFormats = new[] { "zwo", "image" };
        var formats = result.Tokens.Select(t => t.Value.ToLowerInvariant()).ToArray();
        
        var invalidFormats = formats.Where(f => !validFormats.Contains(f)).Distinct().ToArray();
        if (invalidFormats.Any())
        {
            result.ErrorMessage = $"Invalid format(s): {string.Join(", ", invalidFormats)}. Supported formats are: zwo, image.";
            return Array.Empty<string>();
        }
        return formats;
    })
{ AllowMultipleArgumentsPerToken = true };

var dryRunOption = new Option<bool>(
    name: "--dry-run",
    description: "Perform a dry run without generating files (default: false)",
    getDefaultValue: () => false);

var verboseOption = new Option<bool>(
    name: "--verbose",
    description: "Enable verbose logging (default: false). Prints detailed pipeline information.",
    getDefaultValue: () => false);

var quietOption = new Option<bool>(
    name: "--quiet",
    description: "Enable quiet mode (default: false). Only print workflow steps without detailed information.",
    getDefaultValue: () => false);

var noLogoOption = new Option<bool>(
    name: "--no-logo",
    description: "Suppress the display of the program logo (default: false).",
    getDefaultValue: () => false);


rootCommand.AddValidator(commandResult =>
{
    var dryRun = commandResult.GetValueForOption(dryRunOption);
    
    
    try
    {
        var formats = commandResult.GetValueForOption(formatsOption);
        if (!dryRun && (formats == null || formats.Length == 0))
        {
            commandResult.ErrorMessage = "Option --format is required unless --dry-run is specified.";
        }
    }
    catch (InvalidOperationException)
    {
        // Skip if format parsing failed (error already reported)
        return;
    }

});


// Add validator for mutually exclusive verbose/quiet and format requirement
rootCommand.AddValidator(commandResult =>
{
    var verbose = commandResult.GetValueForOption(verboseOption);
    var quiet = commandResult.GetValueForOption(quietOption);
    
    if (verbose && quiet)
    {
        commandResult.ErrorMessage = "Options --verbose and --quiet cannot be used together.";
    }
});

rootCommand.AddOption(inputFileOption);
rootCommand.AddOption(rotationsOption);
rootCommand.AddOption(outputFolderOption);
rootCommand.AddOption(formatsOption);
rootCommand.AddOption(dryRunOption);
rootCommand.AddOption(verboseOption);
rootCommand.AddOption(quietOption);
rootCommand.AddOption(noLogoOption);
rootCommand.SetHandler((inputFile, rotations, outputFolder, formats, dryRun, verbose, quiet, noLogo) =>
{
    try
    {
        // Print header unless --no-logo is specified
        if (!noLogo)
        {
            PrintHeader();
        }

        // Print CLI parameters unless --quiet is specified
        if(!quiet)
        {
            Console.WriteLine("======================================");
            Console.WriteLine("CLI Parameters:");
            Console.WriteLine($"    Input File: {inputFile?.FullName}");
            Console.WriteLine($"    Rotations: {rotations}");
            Console.WriteLine($"    Output Folder: {outputFolder?.FullName}");
            Console.WriteLine($"    Formats ({formats?.Length ?? 0}): {string.Join(", ", formats ?? Array.Empty<string>())}");
            Console.WriteLine($"    Dry Run: {dryRun}");
            Console.WriteLine($"    Verbose: {verbose}");
            Console.WriteLine($"    Quiet: {quiet}");
            Console.WriteLine($"    No Logo: {noLogo}");
            Console.WriteLine("======================================");
            Console.WriteLine();
        }
        
        // Read and parse CSV file
        if(!quiet)
        {
            Console.WriteLine("Reading and parsing CSV file...");
        }
        var csvContent = File.ReadAllText(inputFile!.FullName);
        var parser = new CsvParser();
        var powerPlans = parser.ParseCsv(csvContent);
        if (verbose)
        {
            Console.WriteLine($"Parsed {powerPlans.Count} rider power plans.");
            Console.WriteLine();
            //TODO: Print parsed power plans details
        }


        // Generate paceline workout plan
        if(!quiet)
        {
            Console.WriteLine("Composing paceline plan...");
        }
        var pacelinePlanComposer = new PacelinePlanComposer();
        var plan = pacelinePlanComposer.CreatePlan(powerPlans, rotations);
        if (verbose)
        {
            Console.WriteLine("Generated Paceline Plan:");
            foreach (var pull in plan.Pulls)
            {
                Console.WriteLine($"Pull {pull.PullNumber}, Duration={pull.PullDuration.TotalSeconds}s"); //TODO: add all pull details
                foreach (var position in pull.PacelinePositions)
                {
                    Console.WriteLine($"  Position {position.PositionInPull + 1}: Rider {position.Rider.Name}, Target Power {position.TargetPower} W");
                }
            }

            Console.WriteLine();
        }

        // Export workouts
        if(!quiet)
        {
            Console.WriteLine("Projecting workouts...");
        }
        var projector = new WorkoutProjector();
        var workouts = projector.Project(plan);

        if(verbose)
        {
            Console.WriteLine($"Projected workouts for {workouts.Count} riders.");
            Console.WriteLine();
            //TODO: Print projected workouts details
        }

        if (dryRun)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n⚠️ Dry Run mode enabled - no files will be generated.");
            Console.ResetColor();
            return;
        }

        if(formats == null || formats.Length == 0)
        {
            if(!quiet)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No output formats specified. Terminating without exporting files.");
                Console.ResetColor();
            }
        }
        else
        {
            if(!quiet)
            {
                Console.WriteLine("Exporting workouts to files...");
            }
            foreach (var format in formats)
            {
                switch (format)
                {
                    case "zwo":
                        if(!quiet)
                        {
                            Console.WriteLine("Exporting ZWO files...");
                        }
                        var zwoExporter = new ZwoExporter();
                        zwoExporter.ExportToFiles(workouts, outputFolder?.FullName);
                        if(!quiet)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"✅ ZWO workouts exported to '{outputFolder?.FullName}' directory");
                            Console.ResetColor();
                        }
                        if(verbose)
                        {
                            Console.WriteLine($"   Generated {workouts.Count} ZWO files:");
                            foreach (var riderName in workouts.Keys)
                            {
                                var fileName = ZwoExporter.GetWorkoutFileName(riderName);
                                Console.WriteLine($"   - {fileName}");
                            }
                        }
                        break;
                    case "image":
                        if(!quiet)
                        {
                            Console.WriteLine("Exporting workout images...");
                        }
                        var imageExporter = new ImageExporter();
                        imageExporter.ExportToFiles(workouts, outputFolder?.FullName, powerPlans.Count, powerPlans);
                        if(!quiet)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"✅ Workout images exported to '{outputFolder?.FullName}' directory");
                            Console.ResetColor();
                        }
                        if(verbose)
                        {
                            Console.WriteLine($"   Generated {workouts.Count} PNG files:");
                            foreach (var riderName in workouts.Keys)
                            {
                                var fileName = ImageExporter.GetWorkoutImageFileName(riderName);
                                Console.WriteLine($"   - {fileName}");
                            }
                        }
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error: Unknown format '{format}'. Supported formats are: zwo, image.");
                        Console.ResetColor();
                        break;
                }
            }
            if(!quiet)
            {
                Console.WriteLine();
                Console.WriteLine("✅ All done!");
            }
        }

    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error: {ex.Message}");
        Console.ResetColor();
        Environment.Exit(1);
    }
}, inputFileOption, rotationsOption, outputFolderOption, formatsOption, dryRunOption, verboseOption, quietOption, noLogoOption);

return await new CommandLineBuilder(rootCommand)
    .UseDefaults()  // Adds standard middleware
    .Build()
    .InvokeAsync(args);

void PrintHeader()
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine();
    Console.WriteLine("╔══════════════════════════════════════╗");
    Console.WriteLine("║  Zwift TTT Race Simulator            ║");
    Console.WriteLine("╚══════════════════════════════════════╝");
    Console.WriteLine(" by Simone Chiaretta (c) 2026");
    Console.WriteLine($" Version {Assembly.GetExecutingAssembly().GetName().Version}");
    //TODO: Add more info?
    Console.ResetColor();
    Console.WriteLine();
}