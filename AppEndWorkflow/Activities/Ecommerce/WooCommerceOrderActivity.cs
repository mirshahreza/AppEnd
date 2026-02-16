using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Ecommerce;

/// <summary>
/// Creates order in WooCommerce store.
/// </summary>
[Activity(
    Category = "E-commerce",
    Description = "Create WooCommerce order",
    DisplayName = "WooCommerce Order"
)]
public class WooCommerceOrderActivity : CodeActivity
{
    [Input(Description = "WooCommerce store URL")]
    public Input<string> StoreUrl { get; set; } = default!;

    [Input(Description = "WooCommerce API key")]
    public Input<string> ApiKey { get; set; } = default!;

    [Input(Description = "WooCommerce API secret")]
    public Input<string> ApiSecret { get; set; } = default!;

    [Input(Description = "Order data JSON")]
    public Input<string> OrderData { get; set; } = default!;

    [Output(Description = "WooCommerce order ID")]
    public Output<string> OrderId { get; set; } = default!;

    [Output(Description = "Order total")]
    public Output<decimal> OrderTotal { get; set; } = default!;

    [Output(Description = "Whether creation succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var storeUrl = context.Get(StoreUrl) ?? throw new ArgumentException("StoreUrl is required");
            var apiKey = context.Get(ApiKey) ?? throw new ArgumentException("ApiKey is required");
            var apiSecret = context.Get(ApiSecret) ?? throw new ArgumentException("ApiSecret is required");
            var orderData = context.Get(OrderData) ?? throw new ArgumentException("OrderData is required");

            using var httpClient = new HttpClient();
            var auth = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{apiKey}:{apiSecret}"));
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {auth}");

            var endpoint = $"{storeUrl}/wp-json/wc/v3/orders";
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(orderData, System.Text.Encoding.UTF8, "application/json")
            };

            var response = httpClient.SendAsync(request).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;

            if (root.TryGetProperty("id", out var idProp) && root.TryGetProperty("total", out var totalProp))
            {
                var orderId = idProp.GetString();
                var orderTotal = decimal.Parse(totalProp.GetString() ?? "0");

                context.Set(OrderId, orderId ?? "");
                context.Set(OrderTotal, orderTotal);
                context.Set(Success, true);
                context.Set(Error, null);
            }
            else
            {
                throw new Exception("Failed to create order");
            }
        }
        catch (Exception ex)
        {
            context.Set(OrderId, "");
            context.Set(OrderTotal, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
