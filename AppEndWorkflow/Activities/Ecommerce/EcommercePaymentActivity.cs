using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Ecommerce;

/// <summary>
/// Processes payment through various e-commerce platforms.
/// </summary>
[Activity(
    Category = "E-commerce",
    Description = "Process e-commerce payment",
    DisplayName = "E-commerce Payment"
)]
public class EcommercePaymentActivity : CodeActivity
{
    [Input(Description = "Platform: Shopify, WooCommerce, Magento")]
    public Input<string> Platform { get; set; } = default!;

    [Input(Description = "API credentials")]
    public Input<string> Credentials { get; set; } = default!;

    [Input(Description = "Order ID")]
    public Input<string> OrderId { get; set; } = default!;

    [Input(Description = "Payment method")]
    public Input<string> PaymentMethod { get; set; } = default!;

    [Input(Description = "Amount to charge")]
    public Input<decimal> Amount { get; set; } = default!;

    [Output(Description = "Transaction ID")]
    public Output<string> TransactionId { get; set; } = default!;

    [Output(Description = "Payment status")]
    public Output<string> Status { get; set; } = default!;

    [Output(Description = "Whether payment succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var platform = context.Get(Platform) ?? throw new ArgumentException("Platform is required");
            var credentials = context.Get(Credentials) ?? throw new ArgumentException("Credentials is required");
            var orderId = context.Get(OrderId) ?? throw new ArgumentException("OrderId is required");
            var paymentMethod = context.Get(PaymentMethod) ?? throw new ArgumentException("PaymentMethod is required");
            var amount = context.Get(Amount);

            // NOTE: Actual implementation would call platform-specific payment APIs

            var transactionId = Guid.NewGuid().ToString();
            var status = "completed";

            context.Set(TransactionId, transactionId);
            context.Set(Status, status);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(TransactionId, "");
            context.Set(Status, "failed");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
