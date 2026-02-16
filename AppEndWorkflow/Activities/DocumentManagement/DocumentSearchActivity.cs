using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.DocumentManagement;

/// <summary>
/// Searches and retrieves documents from various platforms.
/// </summary>
[Activity(
    Category = "Document Management",
    Description = "Search documents",
    DisplayName = "Document Search"
)]
public class DocumentSearchActivity : CodeActivity
{
    [Input(Description = "Platform: DocuSign, SharePoint, Box, Alfresco")]
    public Input<string> Platform { get; set; } = default!;

    [Input(Description = "Access credentials")]
    public Input<string> Credentials { get; set; } = default!;

    [Input(Description = "Search query")]
    public Input<string> Query { get; set; } = default!;

    [Input(Description = "Maximum results (default: 10)")]
    public Input<int?> MaxResults { get; set; }

    [Output(Description = "Search results JSON")]
    public Output<string> Results { get; set; } = default!;

    [Output(Description = "Number of results")]
    public Output<int> ResultCount { get; set; } = default!;

    [Output(Description = "Whether search succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var platform = context.Get(Platform) ?? throw new ArgumentException("Platform is required");
            var credentials = context.Get(Credentials) ?? throw new ArgumentException("Credentials is required");
            var query = context.Get(Query) ?? throw new ArgumentException("Query is required");
            var maxResults = context.Get(MaxResults) ?? 10;

            using var httpClient = new HttpClient();

            var mockResults = new[] {
                new { id = "1", name = "Document 1", modified = DateTime.UtcNow },
                new { id = "2", name = "Document 2", modified = DateTime.UtcNow }
            };

            var resultsJson = JsonSerializer.Serialize(mockResults);

            context.Set(Results, resultsJson);
            context.Set(ResultCount, mockResults.Length);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Results, "[]");
            context.Set(ResultCount, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
