namespace ZwiftTTTSim.Core.Exceptions;

public class CsvParseException : Exception
{
    public int LineNumber { get; set; }
    public string LineContent { get; set; }
    public CsvParseException(int lineNumber, string lineContent, string message)
    : base(message)
    {
        LineNumber = lineNumber;
        LineContent = lineContent;
    }
}