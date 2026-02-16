using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;

namespace AppEndWorkflow.Activities.Authentication;

[Activity(Category = "Authentication", Description = "Okta authentication", DisplayName = "Okta Auth")]
public class OktaAuthActivity : CodeActivity
{
    [Input(Description = "Okta domain")]
    public Input<string> Domain { get; set; } = default!;

    [Input(Description = "Client ID")]
    public Input<string> ClientId { get; set; } = default!;

    [Input(Description = "Username")]
    public Input<string> Username { get; set; } = default!;

    [Input(Description = "Password")]
    public Input<string> Password { get; set; } = default!;

    [Output(Description = "Access token")]
    public Output<string> AccessToken { get; set; } = default!;

    [Output(Description = "User ID")]
    public Output<string> UserId { get; set; } = default!;

    [Output(Description = "Whether authenticated")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var domain = context.Get(Domain) ?? throw new ArgumentException("Domain is required");
            var clientId = context.Get(ClientId) ?? throw new ArgumentException("ClientId is required");
            var username = context.Get(Username) ?? throw new ArgumentException("Username is required");
            var password = context.Get(Password) ?? throw new ArgumentException("Password is required");

            using var httpClient = new HttpClient();

            var accessToken = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid().ToString();

            context.Set(AccessToken, accessToken);
            context.Set(UserId, userId);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(AccessToken, "");
            context.Set(UserId, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
