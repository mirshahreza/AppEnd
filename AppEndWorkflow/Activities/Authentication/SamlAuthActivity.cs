using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Authentication;

[Activity(Category = "Authentication", Description = "SAML authentication", DisplayName = "SAML Auth")]
public class SamlAuthActivity : CodeActivity
{
    [Input(Description = "Identity provider URL")]
    public Input<string> IdpUrl { get; set; } = default!;

    [Input(Description = "Service provider ID")]
    public Input<string> SpId { get; set; } = default!;

    [Input(Description = "Assertion consumer service URL")]
    public Input<string> AcsUrl { get; set; } = default!;

    [Output(Description = "SAML request")]
    public Output<string> SamlRequest { get; set; } = default!;

    [Output(Description = "Authorization URL")]
    public Output<string> AuthUrl { get; set; } = default!;

    [Output(Description = "Whether created")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var idpUrl = context.Get(IdpUrl) ?? throw new ArgumentException("IdpUrl is required");
            var spId = context.Get(SpId) ?? throw new ArgumentException("SpId is required");
            var acsUrl = context.Get(AcsUrl) ?? throw new ArgumentException("AcsUrl is required");

            var samlRequest = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"<samlp:AuthnRequest></samlp:AuthnRequest>"));
            var authUrl = $"{idpUrl}?SAMLRequest={System.Net.WebUtility.UrlEncode(samlRequest)}&RelayState=";

            context.Set(SamlRequest, samlRequest);
            context.Set(AuthUrl, authUrl);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(SamlRequest, "");
            context.Set(AuthUrl, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
