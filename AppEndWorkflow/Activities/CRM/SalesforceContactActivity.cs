using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.CRM;

/// <summary>
/// Creates or updates a contact in Salesforce.
/// </summary>
[Activity(
    Category = "CRM",
    Description = "Create/Update Salesforce contact",
    DisplayName = "Salesforce Contact"
)]
public class SalesforceContactActivity : CodeActivity
{
    [Input(Description = "Salesforce instance URL")]
    public Input<string> InstanceUrl { get; set; } = default!;

    [Input(Description = "Access token")]
    public Input<string> AccessToken { get; set; } = default!;

    [Input(Description = "Contact data JSON")]
    public Input<string> ContactData { get; set; } = default!;

    [Input(Description = "Contact ID (optional - for update)")]
    public Input<string?> ContactId { get; set; }

    [Output(Description = "Salesforce contact ID")]
    public Output<string> SalesforceId { get; set; } = default!;

    [Output(Description = "Whether operation succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var instanceUrl = context.Get(InstanceUrl) ?? throw new ArgumentException("InstanceUrl is required");
            var accessToken = context.Get(AccessToken) ?? throw new ArgumentException("AccessToken is required");
            var contactData = context.Get(ContactData) ?? throw new ArgumentException("ContactData is required");
            var contactId = context.Get(ContactId);

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var endpoint = string.IsNullOrWhiteSpace(contactId)
                ? $"{instanceUrl}/services/data/v57.0/sobjects/Contact"
                : $"{instanceUrl}/services/data/v57.0/sobjects/Contact/{contactId}";

            var method = string.IsNullOrWhiteSpace(contactId) ? HttpMethod.Post : new HttpMethod("PATCH");
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
                var sfId = idProp.GetString();
                context.Set(SalesforceId, sfId ?? "");
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
            context.Set(SalesforceId, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
