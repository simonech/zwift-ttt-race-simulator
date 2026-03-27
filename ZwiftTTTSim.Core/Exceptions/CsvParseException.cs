namespace ZwiftTTTSim.Core.Exceptions;

public class CsvParseException : Exception
{
    /// <summary>
    /// Gets the line number in the CSV file where the parsing error occurred.
    /// </summary>
    public int LineNumber { get; init; }
    /// <summary>
    /// Gets the content of the line in the CSV file where the parsing error occurred.
    /// </summary>
    public string? LineContent { get; init; }
    public CsvParseException(int lineNumber, string? lineContent, string message)
    : base(message)
    {
        LineNumber = lineNumber;
        LineContent = lineContent;
    }
}