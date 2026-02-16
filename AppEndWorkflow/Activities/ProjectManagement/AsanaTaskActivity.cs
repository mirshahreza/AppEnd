using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.ProjectManagement;

/// <summary>
/// Creates or updates task in Asana.
/// </summary>
[Activity(
    Category = "Project Management",
    Description = "Create/Update Asana task",
    DisplayName = "Asana Task"
)]
public class AsanaTaskActivity : CodeActivity
{
    [Input(Description = "Asana personal access token")]
    public Input<string> AccessToken { get; set; } = default!;

    [Input(Description = "Project ID")]
    public Input<string> ProjectId { get; set; } = default!;

    [Input(Description = "Task data JSON")]
    public Input<string> TaskData { get; set; } = default!;

    [Input(Description = "Task ID (optional - for update)")]
    public Input<string?> TaskId { get; set; }

    [Output(Description = "Asana task ID")]
    public Output<string> AsanaTaskId { get; set; } = default!;

    [Output(Description = "Task URL")]
    public Output<string> TaskUrl { get; set; } = default!;

    [Output(Description = "Whether operation succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var accessToken = context.Get(AccessToken) ?? throw new ArgumentException("AccessToken is required");
            var projectId = context.Get(ProjectId) ?? throw new ArgumentException("ProjectId is required");
            var taskData = context.Get(TaskData) ?? throw new ArgumentException("TaskData is required");
            var taskId = context.Get(TaskId);

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var endpoint = string.IsNullOrWhiteSpace(taskId)
                ? $"https://app.asana.com/api/1.0/projects/{projectId}/tasks"
                : $"https://app.asana.com/api/1.0/tasks/{taskId}";

            var method = string.IsNullOrWhiteSpace(taskId) ? HttpMethod.Post : HttpMethod.Put;
            var request = new HttpRequestMessage(method, endpoint)
            {
                Content = new StringContent(taskData, System.Text.Encoding.UTF8, "application/json")
            };

            var response = httpClient.SendAsync(request).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;

            if (root.TryGetProperty("data", out var dataProp) && 
                dataProp.TryGetProperty("gid", out var gidProp))
            {
                var asanaTaskId = gidProp.GetString();
                var taskUrl = $"https://app.asana.com/0/{projectId}/{asanaTaskId}";

                context.Set(AsanaTaskId, asanaTaskId ?? "");
                context.Set(TaskUrl, taskUrl);
                context.Set(Success, true);
                context.Set(Error, null);
            }
            else
            {
                throw new Exception("Failed to create/update task");
            }
        }
        catch (Exception ex)
        {
            context.Set(AsanaTaskId, "");
            context.Set(TaskUrl, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
