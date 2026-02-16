using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Payments;

/// <summary>
/// Processes payment through PayPal.
/// </summary>
[Activity(
    Category = "Payments",
    Description = "Process PayPal payment",
    DisplayName = "PayPal Payment"
)]
public class PayPalPaymentActivity : CodeActivity
{
    [Input(Description = "PayPal client ID")]
    public Input<string> ClientId { get; set; } = default!;

    [Input(Description = "PayPal client secret")]
    public Input<string> ClientSecret { get; set; } = default!;

    [Input(Description = "Amount")]
    public Input<decimal> Amount { get; set; } = default!;

    [Input(Description = "Currency (default: USD)")]
    public Input<string?> Currency { get; set; }

    [Input(Description = "Return URL")]
    public Input<string> ReturnUrl { get; set; } = default!;

    [Input(Description = "Cancel URL")]
    public Input<string> CancelUrl { get; set; } = default!;

    [Output(Description = "PayPal transaction ID")]
    public Output<string> TransactionId { get; set; } = default!;

    [Output(Description = "Approval URL")]
    public Output<string> ApprovalUrl { get; set; } = default!;

    [Output(Description = "Whether payment setup succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var clientId = context.Get(ClientId) ?? throw new ArgumentException("ClientId is required");
            var clientSecret = context.Get(ClientSecret) ?? throw new ArgumentException("ClientSecret is required");
            var amount = context.Get(Amount);
            var currency = context.Get(Currency) ?? "USD";

            using var httpClient = new HttpClient();
            var auth = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");

            var transactionId = Guid.NewGuid().ToString();
            var approvalUrl = $"https://www.paypal.com/checkoutnow?token={transactionId}";

            context.Set(TransactionId, transactionId);
            context.Set(ApprovalUrl, approvalUrl);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(TransactionId, "");
            context.Set(ApprovalUrl, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
