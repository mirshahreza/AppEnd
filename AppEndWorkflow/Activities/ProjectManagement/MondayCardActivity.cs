using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.ProjectManagement;

/// <summary>
/// Creates or updates card in Monday.com.
/// </summary>
[Activity(
    Category = "Project Management",
    Description = "Create/Update Monday.com card",
    DisplayName = "Monday.com Card"
)]
public class MondayCardActivity : CodeActivity
{
    [Input(Description = "Monday.com API token")]
    public Input<string> ApiToken { get; set; } = default!;

    [Input(Description = "Board ID")]
    public Input<string> BoardId { get; set; } = default!;

    [Input(Description = "Card data (name, etc.)")]
    public Input<string> CardData { get; set; } = default!;

    [Input(Description = "Item ID (optional - for update)")]
    public Input<string?> ItemId { get; set; }

    [Output(Description = "Monday.com item ID")]
    public Output<string> MondayItemId { get; set; } = default!;

    [Output(Description = "Whether operation succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var apiToken = context.Get(ApiToken) ?? throw new ArgumentException("ApiToken is required");
            var boardId = context.Get(BoardId) ?? throw new ArgumentException("BoardId is required");
            var cardData = context.Get(CardData) ?? throw new ArgumentException("CardData is required");
            var itemId = context.Get(ItemId);

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", apiToken);

            var query = string.IsNullOrWhiteSpace(itemId)
                ? $"mutation {{ create_item(board_id: {boardId}, item_name: \"{cardData}\") {{ id }} }}"
                : $"mutation {{ update_item(item_id: {itemId}, column_values: \\\"{cardData}\\\") {{ id }} }}";

            var payload = new { query };
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.monday.com/v2")
            {
                Content = JsonContent.Create(payload)
            };

            var response = httpClient.SendAsync(request).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;

            if (root.TryGetProperty("data", out var dataProp))
            {
                var key = string.IsNullOrWhiteSpace(itemId) ? "create_item" : "update_item";
                if (dataProp.TryGetProperty(key, out var operationProp) &&
                    operationProp.TryGetProperty("id", out var idProp))
                {
                    var mondayItemId = idProp.GetString();
                    context.Set(MondayItemId, mondayItemId ?? "");
                    context.Set(Success, true);
                    context.Set(Error, null);
                    return;
                }
            }

            throw new Exception("Failed to create/update card");
        }
        catch (Exception ex)
        {
            context.Set(MondayItemId, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
