using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.Authentication;

[Activity(Category = "Authentication", Description = "OAuth authorization flow", DisplayName = "OAuth Flow")]
public class OAuthFlowActivity : CodeActivity
{
    [Input(Description = "OAuth provider")]
    public Input<string> Provider { get; set; } = default!;

    [Input(Description = "Client ID")]
    public Input<string> ClientId { get; set; } = default!;

    [Input(Description = "Client secret")]
    public Input<string> ClientSecret { get; set; } = default!;

    [Input(Description = "Redirect URI")]
    public Input<string> RedirectUri { get; set; } = default!;

    [Input(Description = "Scope")]
    public Input<string> Scope { get; set; } = default!;

    [Output(Description = "Authorization URL")]
    public Output<string> AuthorizationUrl { get; set; } = default!;

    [Output(Description = "Whether initiated")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var provider = context.Get(Provider) ?? throw new ArgumentException("Provider is required");
            var clientId = context.Get(ClientId) ?? throw new ArgumentException("ClientId is required");
            var redirectUri = context.Get(RedirectUri) ?? throw new ArgumentException("RedirectUri is required");
            var scope = context.Get(Scope) ?? throw new ArgumentException("Scope is required");

            var authorizationUrl = $"https://oauth.{provider.ToLower()}.com/authorize?client_id={clientId}&redirect_uri={redirectUri}&scope={scope}&response_type=code";

            context.Set(AuthorizationUrl, authorizationUrl);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(AuthorizationUrl, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
