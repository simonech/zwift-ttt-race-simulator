namespace ZwiftTTTSim.CLI;

/// <summary>
/// Renders data as a formatted ASCII table to the console.
/// </summary>
internal static class AsciiTableRenderer
{
    /// <summary>
    /// Renders and prints an ASCII table with the given headers and rows.
    /// </summary>
    /// <param name="headers">Column header names.</param>
    /// <param name="rows">Rows of string values, each matching the number of headers.</param>
    /// <param name="title">Optional title displayed in a spanning row above the headers.</param>
    public static void PrintTable(string[] headers, IEnumerable<string[]> rows, string? title = null)
    {
        var rowList = rows.ToList();
        var colCount = headers.Length;
        var widths = new int[colCount];

        for (int i = 0; i < colCount; i++)
            widths[i] = headers[i].Length;

        foreach (var row in rowList)
            for (int i = 0; i < colCount; i++)
                if (i < row.Length)
                    widths[i] = Math.Max(widths[i], row[i].Length);

        var separator = "+-" + string.Join("-+-", widths.Select(w => new string('-', w))) + "-+";
        // Inner width = total of all column widths + separators between columns
        var innerWidth = widths.Sum() + (colCount - 1) * 3;

        if (title != null)
        {
            var titleSeparator = "+" + new string('-', innerWidth + 2) + "+";
            Console.WriteLine(titleSeparator);
            Console.WriteLine("| " + title.PadRight(innerWidth) + " |");
        }

        Console.WriteLine(separator);
        Console.WriteLine("| " + string.Join(" | ", headers.Select((h, i) => h.PadRight(widths[i]))) + " |");
        Console.WriteLine(separator);

        foreach (var row in rowList)
        {
            Console.WriteLine("| " + string.Join(" | ", Enumerable.Range(0, colCount)
                .Select(i => (i < row.Length ? row[i] : "").PadRight(widths[i]))) + " |");
        }

        Console.WriteLine(separator);
    }
}
