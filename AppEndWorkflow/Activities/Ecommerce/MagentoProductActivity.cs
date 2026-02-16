using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Ecommerce;

/// <summary>
/// Manages products in Magento store.
/// </summary>
[Activity(
    Category = "E-commerce",
    Description = "Manage Magento product",
    DisplayName = "Magento Product"
)]
public class MagentoProductActivity : CodeActivity
{
    [Input(Description = "Magento store URL")]
    public Input<string> StoreUrl { get; set; } = default!;

    [Input(Description = "Magento API token")]
    public Input<string> ApiToken { get; set; } = default!;

    [Input(Description = "Product data JSON")]
    public Input<string> ProductData { get; set; } = default!;

    [Input(Description = "Product SKU (for update)")]
    public Input<string?> ProductSku { get; set; }

    [Output(Description = "Product ID")]
    public Output<string> ProductId { get; set; } = default!;

    [Output(Description = "Whether operation succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var storeUrl = context.Get(StoreUrl) ?? throw new ArgumentException("StoreUrl is required");
            var apiToken = context.Get(ApiToken) ?? throw new ArgumentException("ApiToken is required");
            var productData = context.Get(ProductData) ?? throw new ArgumentException("ProductData is required");
            var productSku = context.Get(ProductSku);

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiToken}");

            var endpoint = string.IsNullOrWhiteSpace(productSku)
                ? $"{storeUrl}/rest/V1/products"
                : $"{storeUrl}/rest/V1/products/{productSku}";

            var method = string.IsNullOrWhiteSpace(productSku) ? HttpMethod.Post : HttpMethod.Put;
            var request = new HttpRequestMessage(method, endpoint)
            {
                Content = new StringContent(productData, System.Text.Encoding.UTF8, "application/json")
            };

            var response = httpClient.SendAsync(request).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;

            if (root.TryGetProperty("id", out var idProp))
            {
                var productId = idProp.GetString();
                context.Set(ProductId, productId ?? "");
                context.Set(Success, true);
                context.Set(Error, null);
            }
            else
            {
                throw new Exception("Failed to manage product");
            }
        }
        catch (Exception ex)
        {
            context.Set(ProductId, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
