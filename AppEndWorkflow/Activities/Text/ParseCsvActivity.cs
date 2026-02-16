using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Text;

/// <summary>
/// Parses CSV content into JSON array of objects.
/// </summary>
[Activity(
    Category = "Text",
    Description = "Parses CSV to JSON",
    DisplayName = "Parse CSV"
)]
public class ParseCsvActivity : CodeActivity
{
    /// <summary>
    /// CSV text content
    /// </summary>
    [Input(Description = "CSV text content")]
    public Input<string> CsvContent { get; set; } = default!;

    /// <summary>
    /// Column delimiter (default: ",")
    /// </summary>
    [Input(Description = "Column delimiter")]
    public Input<string> Delimiter { get; set; } = new(",");

    /// <summary>
    /// First row is header (default: true)
    /// </summary>
    [Input(Description = "First row is header")]
    public Input<bool> HasHeader { get; set; } = new(true);

    /// <summary>
    /// Text encoding (default: "UTF-8")
    /// </summary>
    [Input(Description = "Text encoding")]
    public Input<string> Encoding { get; set; } = new("UTF-8");

    /// <summary>
    /// JSON array of row objects
    /// </summary>
    [Output(Description = "JSON array of row objects")]
    public Output<string> Result { get; set; } = default!;

    /// <summary>
    /// Number of data rows
    /// </summary>
    [Output(Description = "Number of data rows")]
    public Output<int> RowCount { get; set; } = default!;

    /// <summary>
    /// JSON array of column names
    /// </summary>
    [Output(Description = "JSON array of column names")]
    public Output<string> Columns { get; set; } = default!;

    /// <summary>
    /// Whether parsing succeeded
    /// </summary>
    [Output(Description = "Whether parsing succeeded")]
    public Output<bool> Success { get; set; } = default!;

    /// <summary>
    /// Error message if failed
    /// </summary>
    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var csvContent = context.Get(CsvContent);
            var delimiter = context.Get(Delimiter);
            var hasHeader = context.Get(HasHeader);
            var encodingStr = context.Get(Encoding);

            if (string.IsNullOrWhiteSpace(csvContent))
                throw new ArgumentException("'CsvContent' is required");

            if (string.IsNullOrWhiteSpace(delimiter))
                delimiter = ",";

            // Parse CSV
            var (rows, columns) = ParseCsv(csvContent, delimiter, hasHeader);

            context.Set(Result, JsonSerializer.Serialize(rows));
            context.Set(RowCount, rows.Count);
            context.Set(Columns, JsonSerializer.Serialize(columns));
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Result, "[]");
            context.Set(RowCount, 0);
            context.Set(Columns, "[]");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private (List<Dictionary<string, string>> rows, List<string> columns) ParseCsv(
        string csvContent, string delimiter, bool hasHeader)
    {
        var lines = csvContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var rows = new List<Dictionary<string, string>>();
        var columns = new List<string>();

        if (lines.Length == 0)
            return (rows, columns);

        // Parse header
        var headerLine = lines[0];
        var headerValues = ParseCsvLine(headerLine, delimiter);

        if (hasHeader)
        {
            columns = headerValues;
        }
        else
        {
            // Generate column names
            for (int i = 0; i < headerValues.Count; i++)
                columns.Add($"Column{i + 1}");
        }

        // Parse data rows
        var startIndex = hasHeader ? 1 : 0;
        for (int i = startIndex; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var values = ParseCsvLine(line, delimiter);
            var row = new Dictionary<string, string>();

            for (int j = 0; j < columns.Count && j < values.Count; j++)
            {
                row[columns[j]] = values[j];
            }

            rows.Add(row);
        }

        // If no header, treat first line as data
        if (!hasHeader && lines.Length > 0)
        {
            var firstRow = new Dictionary<string, string>();
            for (int j = 0; j < columns.Count && j < headerValues.Count; j++)
            {
                firstRow[columns[j]] = headerValues[j];
            }
            rows.Insert(0, firstRow);
        }

        return (rows, columns);
    }

    private List<string> ParseCsvLine(string line, string delimiter)
    {
        var values = new List<string>();
        var current = new System.Text.StringBuilder();
        var inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (!inQuotes && line.Substring(i).StartsWith(delimiter))
            {
                values.Add(current.ToString().Trim('"'));
                current.Clear();
                i += delimiter.Length - 1;
            }
            else
            {
                current.Append(c);
            }
        }

        values.Add(current.ToString().Trim('"'));
        return values;
    }
}
