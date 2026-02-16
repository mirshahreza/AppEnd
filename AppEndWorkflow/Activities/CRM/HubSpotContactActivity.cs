using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.CRM;

/// <summary>
/// Creates or updates a contact in HubSpot.
/// </summary>
[Activity(
    Category = "CRM",
    Description = "Create/Update HubSpot contact",
    DisplayName = "HubSpot Contact"
)]
public class HubSpotContactActivity : CodeActivity
{
    [Input(Description = "HubSpot API key")]
    public Input<string> ApiKey { get; set; } = default!;

    [Input(Description = "Contact data JSON")]
    public Input<string> ContactData { get; set; } = default!;

    [Input(Description = "HubSpot contact ID (optional - for update)")]
    public Input<string?> ContactId { get; set; }

    [Output(Description = "HubSpot contact ID")]
    public Output<string> HubSpotId { get; set; } = default!;

    [Output(Description = "Whether operation succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var apiKey = context.Get(ApiKey) ?? throw new ArgumentException("ApiKey is required");
            var contactData = context.Get(ContactData) ?? throw new ArgumentException("ContactData is required");
            var contactId = context.Get(ContactId);

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var endpoint = string.IsNullOrWhiteSpace(contactId)
                ? "https://api.hubapi.com/crm/v3/objects/contacts"
                : $"https://api.hubapi.com/crm/v3/objects/contacts/{contactId}";

            var method = string.IsNullOrWhiteSpace(contactId) ? HttpMethod.Post : HttpMethod.Patch;
            var request = new HttpRequestMessage(method, endpoint)
            {
                Content = new StringContent(contactData, System.Text.Encoding.UTF8, "application/json")
            };

            var response = httpClient.SendAsync(request).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;

            if (root.TryGetProperty("id", out var idProp))
            {
                var hsId = idProp.GetString();
                context.Set(HubSpotId, hsId ?? "");
                context.Set(Success, true);
                context.Set(Error, null);
            }
            else
            {
                throw new Exception("Failed to create/update contact");
            }
        }
        catch (Exception ex)
        {
            context.Set(HubSpotId, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
