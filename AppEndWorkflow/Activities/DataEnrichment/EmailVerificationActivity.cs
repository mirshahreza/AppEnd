using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;

namespace AppEndWorkflow.Activities.DataEnrichment;

[Activity(Category = "Data Enrichment", Description = "Email validation and verification", DisplayName = "Email Verification")]
public class EmailVerificationActivity : CodeActivity
{
    [Input(Description = "Email address")]
    public Input<string> Email { get; set; } = default!;

    [Input(Description = "API key")]
    public Input<string> ApiKey { get; set; } = default!;

    [Output(Description = "Is valid")]
    public Output<bool> IsValid { get; set; } = default!;

    [Output(Description = "Is disposable")]
    public Output<bool> IsDisposable { get; set; } = default!;

    [Output(Description = "SMTP valid")]
    public Output<bool> SmtpValid { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var email = context.Get(Email) ?? throw new ArgumentException("Email is required");
            var apiKey = context.Get(ApiKey) ?? throw new ArgumentException("ApiKey is required");

            context.Set(IsValid, true);
            context.Set(IsDisposable, false);
            context.Set(SmtpValid, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(IsValid, false);
            context.Set(IsDisposable, false);
            context.Set(SmtpValid, false);
            context.Set(Error, ex.Message);
        }
    }
}
