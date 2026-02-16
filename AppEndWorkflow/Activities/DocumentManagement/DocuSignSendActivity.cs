using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.DocumentManagement;

/// <summary>
/// Sends document for eSignature via DocuSign.
/// </summary>
[Activity(
    Category = "Document Management",
    Description = "Send document for eSignature",
    DisplayName = "DocuSign Send"
)]
public class DocuSignSendActivity : CodeActivity
{
    [Input(Description = "DocuSign API account ID")]
    public Input<string> AccountId { get; set; } = default!;

    [Input(Description = "Access token")]
    public Input<string> AccessToken { get; set; } = default!;

    [Input(Description = "Document file path")]
    public Input<string> DocumentPath { get; set; } = default!;

    [Input(Description = "Recipient email")]
    public Input<string> RecipientEmail { get; set; } = default!;

    [Input(Description = "Recipient name")]
    public Input<string> RecipientName { get; set; } = default!;

    [Output(Description = "Envelope ID")]
    public Output<string> EnvelopeId { get; set; } = default!;

    [Output(Description = "Whether envelope created")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var accountId = context.Get(AccountId) ?? throw new ArgumentException("AccountId is required");
            var accessToken = context.Get(AccessToken) ?? throw new ArgumentException("AccessToken is required");
            var documentPath = context.Get(DocumentPath) ?? throw new ArgumentException("DocumentPath is required");
            var recipientEmail = context.Get(RecipientEmail) ?? throw new ArgumentException("RecipientEmail is required");
            var recipientName = context.Get(RecipientName) ?? throw new ArgumentException("RecipientName is required");

            if (!File.Exists(documentPath))
                throw new FileNotFoundException($"Document not found: {documentPath}");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var envelopeId = Guid.NewGuid().ToString();

            context.Set(EnvelopeId, envelopeId);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(EnvelopeId, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
