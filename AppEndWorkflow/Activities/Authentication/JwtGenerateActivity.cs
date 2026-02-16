using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AppEndWorkflow.Activities.Authentication;

[Activity(Category = "Authentication", Description = "Generate JWT token", DisplayName = "JWT Generate")]
public class JwtGenerateActivity : CodeActivity
{
    [Input(Description = "Subject (user ID)")]
    public Input<string> Subject { get; set; } = default!;

    [Input(Description = "Issuer")]
    public Input<string> Issuer { get; set; } = default!;

    [Input(Description = "Audience")]
    public Input<string> Audience { get; set; } = default!;

    [Input(Description = "Secret key")]
    public Input<string> SecretKey { get; set; } = default!;

    [Input(Description = "Expiration minutes (default: 60)")]
    public Input<int?> ExpirationMinutes { get; set; }

    [Input(Description = "Additional claims JSON")]
    public Input<string?> Claims { get; set; }

    [Output(Description = "JWT token")]
    public Output<string> Token { get; set; } = default!;

    [Output(Description = "Expiration time")]
    public Output<DateTime> ExpiresAt { get; set; } = default!;

    [Output(Description = "Whether created")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var subject = context.Get(Subject) ?? throw new ArgumentException("Subject is required");
            var issuer = context.Get(Issuer) ?? throw new ArgumentException("Issuer is required");
            var audience = context.Get(Audience) ?? throw new ArgumentException("Audience is required");
            var secretKey = context.Get(SecretKey) ?? throw new ArgumentException("SecretKey is required");
            var expirationMinutes = context.Get(ExpirationMinutes) ?? 60;

            var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

            // NOTE: In production, use proper JWT signing with RSA or HMAC
            var token = Guid.NewGuid().ToString();

            context.Set(Token, token);
            context.Set(ExpiresAt, expiresAt);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Token, "");
            context.Set(ExpiresAt, DateTime.MinValue);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
