using System.CommandLine;
using ZwiftTTTSim.CLI;
using ZwiftTTTSim.Core.Model;
using ZwiftTTTSim.Core.Services;
using ZwiftTTTSim.Core.Exporters;
using ZwiftTTTSim.Core.Model.Config;
using System.Reflection;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using ZwiftTTTSim.Core.Exceptions;

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
{ IsRequired = true };
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
        return formats.Distinct().ToArray();
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

var raceFileOption = new Option<FileInfo?>(
    name: "--race",
    description: "Path to a JSON file containing the sequence of race segments (overrides --rotations)",
    parseArgument: result =>
    {
        var filePath = result.Tokens.Count > 0 ? result.Tokens[0].Value : null;
        if (filePath == null) return null;

        var fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists)
        {
            result.ErrorMessage = $"Race file '{fileInfo.FullName}' not found.";
            return null;
        }

        return fileInfo;
    });

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


// Add validator for mutually exclusive verbose/quiet options
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
rootCommand.AddOption(raceFileOption);

rootCommand.SetHandler((System.CommandLine.Invocation.InvocationContext context) =>
{
    var inputFile = context.ParseResult.GetValueForOption(inputFileOption);
    var rotations = context.ParseResult.GetValueForOption(rotationsOption);
    var outputFolder = context.ParseResult.GetValueForOption(outputFolderOption);
    var formats = context.ParseResult.GetValueForOption(formatsOption);
    var dryRun = context.ParseResult.GetValueForOption(dryRunOption);
    var verbose = context.ParseResult.GetValueForOption(verboseOption);
    var quiet = context.ParseResult.GetValueForOption(quietOption);
    var noLogo = context.ParseResult.GetValueForOption(noLogoOption);
    var raceFile = context.ParseResult.GetValueForOption(raceFileOption);

    // Mutual Exclusivity Check (#56)
    if (raceFile != null && context.ParseResult.FindResultFor(rotationsOption) != null)
    {
        if (!quiet)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Warning: Both --race and --rotations were provided. The --rotations value will be ignored in favor of the race configuration.");
            Console.ResetColor();
            Console.WriteLine();
        }
    }

    // Print header unless --no-logo is specified
    if (!noLogo)
    {
        PrintHeader();
    }

    // Print CLI parameters unless --quiet is specified
    if (!quiet)
    {
        Console.WriteLine("======================================");
        Console.WriteLine("CLI Parameters:");
        Console.WriteLine($"    Input File: {inputFile?.FullName}");
        if (raceFile != null)
        {
            Console.WriteLine($"    Race File: {raceFile.FullName} (overrides rotations)");
        }
        else
        {
            Console.WriteLine($"    Rotations: {rotations}");
        }
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
    if (!quiet)
    {
        Console.WriteLine("Reading and parsing CSV file...");
    }
    var csvContent = File.ReadAllText(inputFile!.FullName);
    var parser = new CsvParser();
    var powerPlans = new List<RiderPowerPlan>();
    try
    {
        powerPlans = parser.ParseCsv(csvContent);
    }
    catch (CsvParseException ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        if (ex.LineNumber > 0)
        {
            Console.Error.WriteLine($"Error: Invalid CSV at line {ex.LineNumber}:");
            Console.Error.WriteLine(ex.Message);
        }
        else
        {
            Console.Error.WriteLine($"Error: Invalid CSV:");
            Console.Error.WriteLine(ex.Message);
        }
        if (verbose && !string.IsNullOrEmpty(ex.LineContent))
        {
            Console.Error.WriteLine($"Line Content: {ex.LineContent}");
        }
        Console.ResetColor();
        Environment.Exit(1);
    }

    if (verbose)
    {
        Console.WriteLine($"Parsed {powerPlans.Count} rider power plans.");
        Console.WriteLine();
        var headers = new[] { "Name", "FTP (W)", "Duration (s)", "Pos 1 (W)", "Pos 2 (W)", "Pos 3 (W)", "Pos 4+ (W)" };
        var rows = powerPlans.Select(p => new[]
        {
                p.Name,
                p.RiderData.FTP.ToString(),
                p.PullDuration.TotalSeconds.ToString("0"),
                p.PowerByPosition.Length > 0 ? p.PowerByPosition[0].ToString() : "-",
                p.PowerByPosition.Length > 1 ? p.PowerByPosition[1].ToString() : "-",
                p.PowerByPosition.Length > 2 ? p.PowerByPosition[2].ToString() : "-",
                p.PowerByPosition.Length > 3 ? p.PowerByPosition[3].ToString() : "-",
            });
        AsciiTableRenderer.PrintTable(headers, rows);
        Console.WriteLine();
    }

    // Validate input data
    if (!quiet)
    {
        Console.WriteLine("Validating input data...");
    }
    try
    {
        var parsedModelValidator = new ParsedModelValidator();
        parsedModelValidator.ValidateOrThrow(powerPlans);
    }
    catch (ModelValidationException ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine($"Error: Model validation failed:");
        Console.Error.WriteLine(ex.Message);

        if (verbose)
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine("Validation issues:");
            for (int i = 0; i < ex.ValidationErrors.Count; i++)
            {
                Console.Error.WriteLine($"  {i + 1}. {ex.ValidationErrors[i]}");
            }
        }
        else
        {
            Console.Error.WriteLine("Run with --verbose to see validation issue details.");
        }
        Console.ResetColor();
        Environment.Exit(1);
    }

    // Generate paceline workout plan
    var pacelinePlanComposer = new PacelinePlanComposer();
    PacelinePlan plan;
    string? effectiveOutputFolder = outputFolder?.FullName;

    if (raceFile != null)
    {
        if (!quiet)
        {
            Console.WriteLine("Reading and parsing Race JSON file...");
        }
        var raceContent = File.ReadAllText(raceFile.FullName);
        var raceParser = new RaceConfigParser();
        try
        {
            var raceConfig = raceParser.Parse(raceContent);
            
            // Use race name for output folder organization if available
            if (!string.IsNullOrWhiteSpace(raceConfig.Name))
            {
                var sanitizedName = string.Join("_", raceConfig.Name.Split(Path.GetInvalidFileNameChars()));
                effectiveOutputFolder = Path.Combine(effectiveOutputFolder ?? "workouts", sanitizedName);
            }

            if (verbose)
            {
                Console.WriteLine($"Parsed Race Config: {raceConfig.Name} with {raceConfig.Route.Count} segments.");
            }
            if (!quiet)
            {
                Console.WriteLine("Composing paceline plan using race segments...");
            }
            plan = pacelinePlanComposer.CreatePlan(powerPlans, raceConfig.Route);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("Error: Failed to process race configuration:");
            Console.Error.WriteLine(ex.Message);
            Console.ResetColor();
            Environment.Exit(1);
            return;
        }
    }
    else
    {
        if (!quiet)
        {
            Console.WriteLine("Composing paceline plan using fixed rotations...");
        }
        plan = pacelinePlanComposer.CreatePlan(powerPlans, rotations);
    }

    if (verbose)
    {
        Console.WriteLine("Generated Paceline Plan with {0} pulls:", plan.Pulls.Count);
        foreach (var pull in plan.Pulls)
        {
            var pullHeaders = new[] { "Position", "Rider", "Target Power (W)" };
            var pullRows = pull.PacelinePositions.Select(p => new[]
            {
                    (p.PositionInPull + 1).ToString(),
                    p.Rider.Name,
                    p.TargetPower.ToString()
                });
            var pullTitle = $"Pull {pull.PullNumber}, Duration={pull.PullDuration.TotalSeconds}s";
            AsciiTableRenderer.PrintTable(pullHeaders, pullRows, pullTitle);
            Console.WriteLine();
        }
    }

    // Export workouts
    if (!quiet)
    {
        Console.WriteLine("Projecting workouts...");
    }
    var projector = new WorkoutProjector();
    var workouts = projector.Project(plan);

    if (verbose)
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

    if (formats == null || formats.Length == 0)
    {
        if (!quiet)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("No output formats specified. Terminating without exporting files.");
            Console.ResetColor();
        }
    }
    else
    {
        if (!quiet)
        {
            Console.WriteLine("Exporting workouts to files...");
        }
        foreach (var format in formats)
        {
            switch (format)
            {
                case "zwo":
                    if (!quiet)
                    {
                        Console.WriteLine("Exporting ZWO files...");
                    }
                    var zwoExporter = new ZwoExporter();
                    zwoExporter.ExportToFiles(workouts, effectiveOutputFolder);
                    if (!quiet)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"✅ ZWO workouts exported to '{effectiveOutputFolder}' directory");
                        Console.ResetColor();
                    }
                    if (verbose)
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
                    if (!quiet)
                    {
                        Console.WriteLine("Exporting workout images...");
                    }
                    var imageExporter = new ImageExporter();
                    imageExporter.ExportToFiles(workouts, effectiveOutputFolder, powerPlans.Count, powerPlans);
                    if (!quiet)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"✅ Workout images exported to '{effectiveOutputFolder}' directory");
                        Console.ResetColor();
                    }
                    if (verbose)
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
                    Console.Error.WriteLine($"Error: Unknown format '{format}'. Supported formats are: zwo, image.");
                    Console.ResetColor();
                    break;
            }
        }
        if (!quiet)
        {
            Console.WriteLine();
            Console.WriteLine("✅ All done!");
        }
    }
});

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