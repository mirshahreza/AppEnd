using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.CRM;

/// <summary>
/// Creates or updates a contact in Zoho CRM.
/// </summary>
[Activity(
    Category = "CRM",
    Description = "Create/Update Zoho contact",
    DisplayName = "Zoho Contact"
)]
public class ZohoContactActivity : CodeActivity
{
    [Input(Description = "Zoho API key")]
    public Input<string> ApiKey { get; set; } = default!;

    [Input(Description = "Zoho organization ID")]
    public Input<string> OrgId { get; set; } = default!;

    [Input(Description = "Contact data JSON")]
    public Input<string> ContactData { get; set; } = default!;

    [Input(Description = "Zoho contact ID (optional - for update)")]
    public Input<string?> ContactId { get; set; }

    [Output(Description = "Zoho contact ID")]
    public Output<string> ZohoId { get; set; } = default!;

    [Output(Description = "Whether operation succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var apiKey = context.Get(ApiKey) ?? throw new ArgumentException("ApiKey is required");
            var orgId = context.Get(OrgId) ?? throw new ArgumentException("OrgId is required");
            var contactData = context.Get(ContactData) ?? throw new ArgumentException("ContactData is required");
            var contactId = context.Get(ContactId);

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Zoho-oauthtoken {apiKey}");

            var endpoint = string.IsNullOrWhiteSpace(contactId)
                ? $"https://www.zohoapis.com/crm/v4/Contacts"
                : $"https://www.zohoapis.com/crm/v4/Contacts/{contactId}";

            var method = string.IsNullOrWhiteSpace(contactId) ? HttpMethod.Post : HttpMethod.Put;
            var request = new HttpRequestMessage(method, endpoint)
            {
                Content = new StringContent(contactData, System.Text.Encoding.UTF8, "application/json")
            };

            var response = httpClient.SendAsync(request).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;

            if (root.TryGetProperty("data", out var dataProp) && dataProp.ValueKind == JsonValueKind.Array)
            {
                var firstItem = dataProp.EnumerateArray().FirstOrDefault();
                if (firstItem.TryGetProperty("id", out var idProp))
                {
                    var zohoId = idProp.GetString();
                    context.Set(ZohoId, zohoId ?? "");
                    context.Set(Success, true);
                    context.Set(Error, null);
                    return;
                }
            }

            throw new Exception("Failed to create/update contact");
        }
        catch (Exception ex)
        {
            context.Set(ZohoId, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
