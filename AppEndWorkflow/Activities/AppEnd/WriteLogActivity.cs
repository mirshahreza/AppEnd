using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using AppEndCommon;

namespace AppEndWorkflow.Activities.AppEnd;

/// <summary>
/// Writes a log entry to AppEnd's logging system.
/// Uses AppEnd's existing logging infrastructure via ExtensionsForLogging methods.
/// </summary>
[Activity(
    Category = "AppEnd",
    Description = "Writes a log entry to AppEnd's logging system",
    DisplayName = "Write Log"
)]
public class WriteLogActivity : CodeActivity
{
    /// <summary>
    /// Log level: "Info", "Warning", "Error"
    /// </summary>
    [Input(Description = "Log level: 'Info', 'Warning', 'Error'")]
    public Input<string> Level { get; set; } = default!;

    /// <summary>
    /// Log message
    /// </summary>
    [Input(Description = "Log message")]
    public Input<string> Message { get; set; } = default!;

    /// <summary>
    /// Source identifier (e.g., "Workflow:order-approval")
    /// </summary>
    [Input(Description = "Source identifier (e.g., 'Workflow:order-approval')")]
    public Input<string> Source { get; set; } = default!;

    /// <summary>
    /// Optional JSON details
    /// </summary>
    [Input(Description = "Optional JSON details")]
    public Input<string?> Details { get; set; }

    /// <summary>
    /// Whether the log was written successfully
    /// </summary>
    [Output(Description = "Whether the log was written successfully")]
    public Output<bool> Success { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var level = context.Get(Level);
            var message = context.Get(Message);
            var source = context.Get(Source);
            var details = context.Get(Details);

            if (string.IsNullOrWhiteSpace(level))
                throw new ArgumentException("'Level' is required");

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("'Message' is required");

            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentException("'Source' is required");

            // Log using AppEnd's logging system
            var logMessage = $"[{source}] {message}";
            if (!string.IsNullOrWhiteSpace(details))
                logMessage += $" | Details: {details}";

            switch (level.ToLower())
            {
                case "info":
                    LogMan.LogConsole(logMessage);
                    break;

                case "warning":
                case "warn":
                    LogMan.LogWarning(logMessage);
                    break;

                case "error":
                    LogMan.LogError(logMessage);
                    break;

                default:
                    LogMan.LogConsole(logMessage);
                    break;
            }

            context.Set(Success, true);
        }
        catch (Exception ex)
        {
            LogMan.LogError($"WriteLogActivity failed: {ex.Message}");
            context.Set(Success, false);
        }
    }
}
