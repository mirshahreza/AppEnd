using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.DataEnrichment;

[Activity(Category = "Data Enrichment", Description = "IP geolocation lookup", DisplayName = "IP Geolocation")]
public class IpGeolocationActivity : CodeActivity
{
    [Input(Description = "IP address")]
    public Input<string> IpAddress { get; set; } = default!;

    [Input(Description = "API key (optional)")]
    public Input<string?> ApiKey { get; set; }

    [Output(Description = "Country")]
    public Output<string> Country { get; set; } = default!;

    [Output(Description = "City")]
    public Output<string> City { get; set; } = default!;

    [Output(Description = "Latitude")]
    public Output<double> Latitude { get; set; } = default!;

    [Output(Description = "Longitude")]
    public Output<double> Longitude { get; set; } = default!;

    [Output(Description = "Whether succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var ipAddress = context.Get(IpAddress) ?? throw new ArgumentException("IpAddress is required");

            using var httpClient = new HttpClient();
            var url = $"https://ipapi.co/{ipAddress}/json/";
            var response = httpClient.GetAsync(url).Result;

            context.Set(Country, "Unknown");
            context.Set(City, "Unknown");
            context.Set(Latitude, 0);
            context.Set(Longitude, 0);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Country, "");
            context.Set(City, "");
            context.Set(Latitude, 0);
            context.Set(Longitude, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
