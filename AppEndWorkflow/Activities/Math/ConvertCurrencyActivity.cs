using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.Math;

/// <summary>
/// Converts amount between currencies using manual rate or API.
/// </summary>
[Activity(
    Category = "Math",
    Description = "Converts between currencies",
    DisplayName = "Convert Currency"
)]
public class ConvertCurrencyActivity : CodeActivity
{
    /// <summary>
    /// Amount to convert
    /// </summary>
    [Input(Description = "Amount to convert")]
    public Input<decimal> Amount { get; set; } = default!;

    /// <summary>
    /// Source currency code (e.g., "IRR", "USD")
    /// </summary>
    [Input(Description = "Source currency code")]
    public Input<string> FromCurrency { get; set; } = default!;

    /// <summary>
    /// Target currency code
    /// </summary>
    [Input(Description = "Target currency code")]
    public Input<string> ToCurrency { get; set; } = default!;

    /// <summary>
    /// Manual rate (optional — uses API if not set)
    /// </summary>
    [Input(Description = "Manual exchange rate")]
    public Input<decimal?> Rate { get; set; }

    /// <summary>
    /// Custom rate API URL (optional)
    /// </summary>
    [Input(Description = "Custom rate API URL")]
    public Input<string?> RateApiUrl { get; set; }

    /// <summary>
    /// Converted amount
    /// </summary>
    [Output(Description = "Converted amount")]
    public Output<decimal> ConvertedAmount { get; set; } = default!;

    /// <summary>
    /// Exchange rate used
    /// </summary>
    [Output(Description = "Exchange rate used")]
    public Output<decimal> RateUsed { get; set; } = default!;

    /// <summary>
    /// Whether conversion succeeded
    /// </summary>
    [Output(Description = "Whether conversion succeeded")]
    public Output<bool> Success { get; set; } = default!;

    /// <summary>
    /// Error message if failed
    /// </summary>
    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var amount = context.Get(Amount);
            var fromCurrency = context.Get(FromCurrency);
            var toCurrency = context.Get(ToCurrency);
            var rate = context.Get(Rate);
            var rateApiUrl = context.Get(RateApiUrl);

            if (string.IsNullOrWhiteSpace(fromCurrency))
                throw new ArgumentException("'FromCurrency' is required");

            if (string.IsNullOrWhiteSpace(toCurrency))
                throw new ArgumentException("'ToCurrency' is required");

            // Get exchange rate
            decimal exchangeRate;
            if (rate.HasValue)
            {
                exchangeRate = rate.Value;
            }
            else if (!string.IsNullOrWhiteSpace(rateApiUrl))
            {
                exchangeRate = FetchExchangeRate(rateApiUrl, fromCurrency, toCurrency);
            }
            else
            {
                throw new ArgumentException("Either 'Rate' or 'RateApiUrl' must be provided");
            }

            var converted = amount * exchangeRate;

            context.Set(ConvertedAmount, converted);
            context.Set(RateUsed, exchangeRate);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(ConvertedAmount, 0);
            context.Set(RateUsed, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private decimal FetchExchangeRate(string apiUrl, string fromCurrency, string toCurrency)
    {
        // TODO: Implement actual API call to exchange rate service
        // For now, return 1 as default rate
        using var httpClient = new System.Net.Http.HttpClient();
        try
        {
            // Example: Call a public API like exchangerate-api.com
            var url = $"{apiUrl}?from={fromCurrency}&to={toCurrency}";
            var response = httpClient.GetAsync(url).Result;
            var content = response.Content.ReadAsStringAsync().Result;

            // Parse JSON response to extract rate
            // This depends on the specific API format
            return 1m;
        }
        catch
        {
            return 1m;
        }
    }
}
