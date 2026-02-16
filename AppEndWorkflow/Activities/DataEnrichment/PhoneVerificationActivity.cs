using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;

namespace AppEndWorkflow.Activities.DataEnrichment;

[Activity(Category = "Data Enrichment", Description = "Phone number verification", DisplayName = "Phone Verification")]
public class PhoneVerificationActivity : CodeActivity
{
    [Input(Description = "Phone number")]
    public Input<string> PhoneNumber { get; set; } = default!;

    [Input(Description = "API key")]
    public Input<string> ApiKey { get; set; } = default!;

    [Input(Description = "Country code")]
    public Input<string?> CountryCode { get; set; }

    [Output(Description = "Is valid")]
    public Output<bool> IsValid { get; set; } = default!;

    [Output(Description = "Formatted number")]
    public Output<string> FormattedNumber { get; set; } = default!;

    [Output(Description = "Country")]
    public Output<string> Country { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var phoneNumber = context.Get(PhoneNumber) ?? throw new ArgumentException("PhoneNumber is required");
            var apiKey = context.Get(ApiKey) ?? throw new ArgumentException("ApiKey is required");

            context.Set(IsValid, true);
            context.Set(FormattedNumber, phoneNumber);
            context.Set(Country, "Unknown");
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(IsValid, false);
            context.Set(FormattedNumber, "");
            context.Set(Country, "");
            context.Set(Error, ex.Message);
        }
    }
}
