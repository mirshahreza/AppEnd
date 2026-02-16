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
/// Executes a SQL command (INSERT/UPDATE/DELETE/EXEC) and returns affected rows count.
/// Uses database configuration from AppEnd's DbConf system.
/// </summary>
[Activity(
    Category = "Database",
    Description = "Executes a SQL command (INSERT/UPDATE/DELETE/EXEC)",
    DisplayName = "Run SQL Command"
)]
public class RunSqlCommandActivity : CodeActivity
{
    /// <summary>
    /// Database config name from AppEnd.DbServers[]
    /// </summary>
    [Input(Description = "Database config name from AppEnd.DbServers[]")]
    public Input<string> DbConfName { get; set; } = default!;

    /// <summary>
    /// SQL command (INSERT / UPDATE / DELETE / EXEC)
    /// </summary>
    [Input(Description = "SQL command (INSERT / UPDATE / DELETE / EXEC)")]
    public Input<string> Command { get; set; } = default!;

    /// <summary>
    /// JSON object of command parameters
    /// </summary>
    [Input(Description = "JSON object of command parameters (optional)")]
    public Input<string?> Parameters { get; set; }

    /// <summary>
    /// Number of rows affected
    /// </summary>
    [Output(Description = "Number of rows affected")]
    public Output<int> AffectedRows { get; set; } = default!;

    /// <summary>
    /// Whether the command executed successfully
    /// </summary>
    [Output(Description = "Whether the command executed successfully")]
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
            var command = context.Get(Command);
            var parametersJson = context.Get(Parameters);

            if (string.IsNullOrWhiteSpace(dbConfName))
                throw new ArgumentException("'DbConfName' is required");

            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentException("'Command' is required");

            // Get database configuration
            var dbConf = DbConf.FromSettings(dbConfName);
            if (dbConf == null)
                throw new InvalidOperationException($"Database configuration '{dbConfName}' not found");

            // Execute command
            var affectedRows = ExecuteCommand(dbConf.ConnectionString, command, parametersJson);

            context.Set(AffectedRows, affectedRows);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(AffectedRows, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private int ExecuteCommand(string connectionString, string command, string? parametersJson)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        using var sqlCommand = new SqlCommand(command, connection);
        sqlCommand.CommandTimeout = 300; // 5 minutes timeout

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

                    sqlCommand.Parameters.AddWithValue($"@{property.Name}", value);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse parameters: {ex.Message}");
            }
        }

        var affectedRows = sqlCommand.ExecuteNonQuery();
        return affectedRows;
    }
}
