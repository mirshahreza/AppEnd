using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Ecommerce;

/// <summary>
/// Creates or updates a product in Shopify.
/// </summary>
[Activity(
    Category = "E-commerce",
    Description = "Create/Update Shopify product",
    DisplayName = "Shopify Product"
)]
public class ShopifyProductActivity : CodeActivity
{
    [Input(Description = "Shopify store URL")]
    public Input<string> StoreUrl { get; set; } = default!;

    [Input(Description = "Shopify access token")]
    public Input<string> AccessToken { get; set; } = default!;

    [Input(Description = "Product data JSON")]
    public Input<string> ProductData { get; set; } = default!;

    [Input(Description = "Product ID (optional - for update)")]
    public Input<string?> ProductId { get; set; }

    [Output(Description = "Shopify product ID")]
    public Output<string> ShopifyId { get; set; } = default!;

    [Output(Description = "Product URL")]
    public Output<string> ProductUrl { get; set; } = default!;

    [Output(Description = "Whether operation succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var storeUrl = context.Get(StoreUrl) ?? throw new ArgumentException("StoreUrl is required");
            var accessToken = context.Get(AccessToken) ?? throw new ArgumentException("AccessToken is required");
            var productData = context.Get(ProductData) ?? throw new ArgumentException("ProductData is required");
            var productId = context.Get(ProductId);

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", accessToken);

            var endpoint = string.IsNullOrWhiteSpace(productId)
                ? $"{storeUrl}/admin/api/2024-01/products.json"
                : $"{storeUrl}/admin/api/2024-01/products/{productId}.json";

            var method = string.IsNullOrWhiteSpace(productId) ? HttpMethod.Post : HttpMethod.Put;
            var request = new HttpRequestMessage(method, endpoint)
            {
                Content = new StringContent(productData, System.Text.Encoding.UTF8, "application/json")
            };

            var response = httpClient.SendAsync(request).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;

            if (root.TryGetProperty("product", out var productProp) &&
                productProp.TryGetProperty("id", out var idProp))
            {
                var shopifyId = idProp.GetInt64().ToString();
                var productUrl = $"{storeUrl}/admin/products/{shopifyId}";

                context.Set(ShopifyId, shopifyId);
                context.Set(ProductUrl, productUrl);
                context.Set(Success, true);
                context.Set(Error, null);
            }
            else
            {
                throw new Exception("Failed to create/update product");
            }
        }
        catch (Exception ex)
        {
            context.Set(ShopifyId, "");
            context.Set(ProductUrl, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
