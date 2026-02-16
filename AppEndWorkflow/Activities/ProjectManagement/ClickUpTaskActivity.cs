using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.ProjectManagement;

/// <summary>
/// Creates or updates task in ClickUp.
/// </summary>
[Activity(
    Category = "Project Management",
    Description = "Create/Update ClickUp task",
    DisplayName = "ClickUp Task"
)]
public class ClickUpTaskActivity : CodeActivity
{
    [Input(Description = "ClickUp API token")]
    public Input<string> ApiToken { get; set; } = default!;

    [Input(Description = "List ID")]
    public Input<string> ListId { get; set; } = default!;

    [Input(Description = "Task data JSON")]
    public Input<string> TaskData { get; set; } = default!;

    [Input(Description = "Task ID (optional - for update)")]
    public Input<string?> TaskId { get; set; }

    [Output(Description = "ClickUp task ID")]
    public Output<string> ClickUpTaskId { get; set; } = default!;

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
            var apiToken = context.Get(ApiToken) ?? throw new ArgumentException("ApiToken is required");
            var listId = context.Get(ListId) ?? throw new ArgumentException("ListId is required");
            var taskData = context.Get(TaskData) ?? throw new ArgumentException("TaskData is required");
            var taskId = context.Get(TaskId);

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", apiToken);

            var endpoint = string.IsNullOrWhiteSpace(taskId)
                ? $"https://api.clickup.com/api/v2/list/{listId}/task"
                : $"https://api.clickup.com/api/v2/task/{taskId}";

            var method = string.IsNullOrWhiteSpace(taskId) ? HttpMethod.Post : HttpMethod.Put;
            var request = new HttpRequestMessage(method, endpoint)
            {
                Content = new StringContent(taskData, System.Text.Encoding.UTF8, "application/json")
            };

            var response = httpClient.SendAsync(request).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;

            if (root.TryGetProperty("id", out var idProp))
            {
                var clickUpTaskId = idProp.GetString();
                var taskUrl = $"https://app.clickup.com/t/{clickUpTaskId}";

                context.Set(ClickUpTaskId, clickUpTaskId ?? "");
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
            context.Set(ClickUpTaskId, "");
            context.Set(TaskUrl, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
