using System.Globalization;
using System.Text;
using System.Xml;
using System.IO;
using System.Linq;
using ZwiftTTTSim.Core.Model;

namespace ZwiftTTTSim.Core.Services;

public class ZwoExporter
{
    public static string GetWorkoutFileName(string riderName)
    {
        ArgumentNullException.ThrowIfNull(riderName);
        
        // Sanitize the rider name to create a valid filename
        var invalidCharsSet = new HashSet<char>(Path.GetInvalidFileNameChars());
        var sanitizedName = string.Concat(riderName.Select(c => invalidCharsSet.Contains(c) ? '_' : c));
        return $"{sanitizedName}_TTT_Workout.zwo";
    }

    public string ExportToZwo(string riderName, List<WorkoutStep> steps)
    {
        ArgumentNullException.ThrowIfNull(riderName);
        ArgumentNullException.ThrowIfNull(steps);
        
        var settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "    ",
            Encoding = Encoding.UTF8,
            OmitXmlDeclaration = false
        };

        using var stringWriter = new StringWriter();
        using (var writer = XmlWriter.Create(stringWriter, settings))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("workout_file");

            writer.WriteElementString("name", $"TTT simulation for {riderName}");

            writer.WriteStartElement("workout");
            foreach (var step in steps)
            {
                writer.WriteStartElement("SteadyState");
                writer.WriteAttributeString("Duration", step.DurationSeconds.ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("Power", step.Intensity.ToString("F2", CultureInfo.InvariantCulture));
                writer.WriteEndElement(); // SteadyState
            }
            writer.WriteEndElement(); // workout

            writer.WriteEndElement(); // workout_file
            writer.WriteEndDocument();
        }

        return stringWriter.ToString();
    }

    public void ExportToFiles(Dictionary<string, List<WorkoutStep>> workouts, string outputDirectory)
    {
        ArgumentNullException.ThrowIfNull(workouts);
        ArgumentNullException.ThrowIfNull(outputDirectory);
        
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        foreach (var (riderName, steps) in workouts)
        {
            var zwoContent = ExportToZwo(riderName, steps);
            var fileName = GetWorkoutFileName(riderName);
            var filePath = Path.Combine(outputDirectory, fileName);
            File.WriteAllText(filePath, zwoContent);
        }
    }
}
