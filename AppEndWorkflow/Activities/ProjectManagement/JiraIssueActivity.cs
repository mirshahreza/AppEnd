using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.ProjectManagement;

/// <summary>
/// Creates or updates Jira issue.
/// </summary>
[Activity(
    Category = "Project Management",
    Description = "Create/Update Jira issue",
    DisplayName = "Jira Issue"
)]
public class JiraIssueActivity : CodeActivity
{
    [Input(Description = "Jira instance URL")]
    public Input<string> InstanceUrl { get; set; } = default!;

    [Input(Description = "Jira username")]
    public Input<string> Username { get; set; } = default!;

    [Input(Description = "Jira API token")]
    public Input<string> ApiToken { get; set; } = default!;

    [Input(Description = "Issue data JSON")]
    public Input<string> IssueData { get; set; } = default!;

    [Input(Description = "Issue key (optional - for update)")]
    public Input<string?> IssueKey { get; set; }

    [Output(Description = "Jira issue key")]
    public Output<string> JiraKey { get; set; } = default!;

    [Output(Description = "Issue URL")]
    public Output<string> IssueUrl { get; set; } = default!;

    [Output(Description = "Whether operation succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var instanceUrl = context.Get(InstanceUrl) ?? throw new ArgumentException("InstanceUrl is required");
            var username = context.Get(Username) ?? throw new ArgumentException("Username is required");
            var apiToken = context.Get(ApiToken) ?? throw new ArgumentException("ApiToken is required");
            var issueData = context.Get(IssueData) ?? throw new ArgumentException("IssueData is required");
            var issueKey = context.Get(IssueKey);

            using var httpClient = new HttpClient();
            var auth = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{apiToken}"));
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");

            var endpoint = string.IsNullOrWhiteSpace(issueKey)
                ? $"{instanceUrl}/rest/api/3/issue"
                : $"{instanceUrl}/rest/api/3/issue/{issueKey}";

            var method = string.IsNullOrWhiteSpace(issueKey) ? HttpMethod.Post : HttpMethod.Put;
            var request = new HttpRequestMessage(method, endpoint)
            {
                Content = new StringContent(issueData, System.Text.Encoding.UTF8, "application/json")
            };

            var response = httpClient.SendAsync(request).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;

            if (root.TryGetProperty("key", out var keyProp))
            {
                var jiraKey = keyProp.GetString();
                var issueUrl = $"{instanceUrl}/browse/{jiraKey}";

                context.Set(JiraKey, jiraKey ?? "");
                context.Set(IssueUrl, issueUrl);
                context.Set(Success, true);
                context.Set(Error, null);
            }
            else if (root.TryGetProperty("id", out var idProp))
            {
                var issueId = idProp.GetString();
                context.Set(JiraKey, issueId ?? "");
                context.Set(IssueUrl, $"{instanceUrl}/browse/{issueId}");
                context.Set(Success, true);
                context.Set(Error, null);
            }
            else
            {
                throw new Exception("Failed to create/update issue");
            }
        }
        catch (Exception ex)
        {
            context.Set(JiraKey, "");
            context.Set(IssueUrl, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
