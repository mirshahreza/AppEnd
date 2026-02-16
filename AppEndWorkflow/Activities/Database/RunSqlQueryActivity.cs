using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using AppEndDbIO;

namespace AppEndWorkflow.Activities.Database;

/// <summary>
/// Executes a SQL SELECT query and returns results as JSON.
/// Uses database configuration from AppEnd's DbConf system.
/// </summary>
[Activity(
    Category = "Database",
    Description = "Executes a SQL SELECT query and returns results as JSON",
    DisplayName = "Run SQL Query"
)]
public class RunSqlQueryActivity : CodeActivity
{
    /// <summary>
    /// Database config name from AppEnd.DbServers[] (e.g., "AppDB")
    /// </summary>
    [Input(Description = "Database config name from AppEnd.DbServers[]")]
    public Input<string> DbConfName { get; set; } = default!;

    /// <summary>
    /// SQL SELECT query
    /// </summary>
    [Input(Description = "SQL SELECT query")]
    public Input<string> Query { get; set; } = default!;

    /// <summary>
    /// JSON object of query parameters for parameterized queries
    /// </summary>
    [Input(Description = "JSON object of query parameters (optional)")]
    public Input<string?> Parameters { get; set; }

    /// <summary>
    /// JSON array of result rows
    /// </summary>
    [Output(Description = "JSON array of result rows")]
    public Output<string> Result { get; set; } = default!;

    /// <summary>
    /// Number of rows returned
    /// </summary>
    [Output(Description = "Number of rows returned")]
    public Output<int> RowCount { get; set; } = default!;

    /// <summary>
    /// Whether the query executed successfully
    /// </summary>
    [Output(Description = "Whether the query executed successfully")]
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
            var dbConfName = context.Get(DbConfName);
            var query = context.Get(Query);
            var parametersJson = context.Get(Parameters);

            if (string.IsNullOrWhiteSpace(dbConfName))
                throw new ArgumentException("'DbConfName' is required");

            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("'Query' is required");

            // Get database configuration
            var dbConf = DbConf.FromSettings(dbConfName);
            if (dbConf == null)
                throw new InvalidOperationException($"Database configuration '{dbConfName}' not found");

            // Execute query and get results
            var (resultJson, rowCount) = ExecuteQuery(dbConf.ConnectionString, query, parametersJson);

            context.Set(Result, resultJson);
            context.Set(RowCount, rowCount);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Result, "[]");
            context.Set(RowCount, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private (string resultJson, int rowCount) ExecuteQuery(string connectionString, string query, string? parametersJson)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        using var command = new SqlCommand(query, connection);
        command.CommandTimeout = 300; // 5 minutes timeout

        // Add parameters if provided
        if (!string.IsNullOrWhiteSpace(parametersJson))
        {
            try
            {
                using var doc = JsonDocument.Parse(parametersJson);
                foreach (var property in doc.RootElement.EnumerateObject())
                {
                    var value = property.Value.ValueKind switch
                    {
                        JsonValueKind.String => (object?)(property.Value.GetString() ?? string.Empty),
                        JsonValueKind.Number => property.Value.GetDecimal(),
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        JsonValueKind.Null => DBNull.Value,
                        _ => property.Value.GetString() ?? string.Empty
                    } ?? DBNull.Value;

                    command.Parameters.AddWithValue($"@{property.Name}", value);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse parameters: {ex.Message}");
            }
        }

        using var reader = command.ExecuteReader();
        var results = new List<Dictionary<string, object>>();

        while (reader.Read())
        {
            var row = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var fieldName = reader.GetName(i);
                var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                row[fieldName] = value!;
            }
            results.Add(row);
        }

        // Convert to JSON
        var resultJson = JsonSerializer.Serialize(results);
        return (resultJson, results.Count);
    }
}
