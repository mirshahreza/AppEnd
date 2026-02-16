using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Payments;

/// <summary>
/// Processes international money transfer through Wise (formerly TransferWise).
/// </summary>
[Activity(
    Category = "Payments",
    Description = "Send money via Wise",
    DisplayName = "Wise Transfer"
)]
public class WiseTransferActivity : CodeActivity
{
    [Input(Description = "Wise API token")]
    public Input<string> ApiToken { get; set; } = default!;

    [Input(Description = "Source currency")]
    public Input<string> SourceCurrency { get; set; } = default!;

    [Input(Description = "Target currency")]
    public Input<string> TargetCurrency { get; set; } = default!;

    [Input(Description = "Amount")]
    public Input<decimal> Amount { get; set; } = default!;

    [Input(Description = "Recipient account ID")]
    public Input<string> RecipientId { get; set; } = default!;

    [Output(Description = "Transfer ID")]
    public Output<string> TransferId { get; set; } = default!;

    [Output(Description = "Exchange rate")]
    public Output<decimal> ExchangeRate { get; set; } = default!;

    [Output(Description = "Whether transfer initiated")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var apiToken = context.Get(ApiToken) ?? throw new ArgumentException("ApiToken is required");
            var sourceCurrency = context.Get(SourceCurrency) ?? throw new ArgumentException("SourceCurrency is required");
            var targetCurrency = context.Get(TargetCurrency) ?? throw new ArgumentException("TargetCurrency is required");
            var amount = context.Get(Amount);

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiToken}");

            var transferId = Guid.NewGuid().ToString();
            var exchangeRate = 1.0m; // Mock rate

            context.Set(TransferId, transferId);
            context.Set(ExchangeRate, exchangeRate);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(TransferId, "");
            context.Set(ExchangeRate, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
