using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Security.Cryptography;

namespace AppEndWorkflow.Activities.Security;

/// <summary>
/// Generates various types of tokens: GUID, Random, Numeric, JWT.
/// </summary>
[Activity(
    Category = "Security",
    Description = "Generates tokens",
    DisplayName = "Generate Token"
)]
public class GenerateTokenActivity : CodeActivity
{
    /// <summary>
    /// Token type: "Guid", "Random", "Numeric", "JWT"
    /// </summary>
    [Input(Description = "Token type: Guid, Random, Numeric, JWT")]
    public Input<string> Type { get; set; } = default!;

    /// <summary>
    /// Length for Random/Numeric types (default: 32)
    /// </summary>
    [Input(Description = "Length for Random/Numeric types")]
    public Input<int> Length { get; set; } = new(32);

    /// <summary>
    /// JSON payload for JWT type
    /// </summary>
    [Input(Description = "JSON payload for JWT type")]
    public Input<string?> JwtPayload { get; set; }

    /// <summary>
    /// JWT expiration in minutes (default: 60)
    /// </summary>
    [Input(Description = "JWT expiration in minutes")]
    public Input<int> ExpirationMinutes { get; set; } = new(60);

    /// <summary>
    /// Generated token
    /// </summary>
    [Output(Description = "Generated token")]
    public Output<string> Token { get; set; } = default!;

    /// <summary>
    /// Expiration time (for JWT)
    /// </summary>
    [Output(Description = "Expiration time")]
    public Output<DateTime?> ExpiresAt { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var type = context.Get(Type);
            var length = context.Get(Length);
            var jwtPayload = context.Get(JwtPayload);
            var expirationMinutes = context.Get(ExpirationMinutes);

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("'Type' is required");

            string token;
            DateTime? expiresAt = null;

            switch (type.ToLower())
            {
                case "guid":
                    token = Guid.NewGuid().ToString();
                    break;

                case "random":
                    token = GenerateRandomToken(length);
                    break;

                case "numeric":
                    token = GenerateNumericToken(length);
                    break;

                case "jwt":
                    // TODO: Implement JWT generation
                    // This requires System.IdentityModel.Tokens.Jwt
                    throw new NotImplementedException("JWT generation requires additional configuration");

                default:
                    throw new NotSupportedException($"Token type '{type}' is not supported");
            }

            context.Set(Token, token);
            context.Set(ExpiresAt, expiresAt);
        }
        catch (Exception ex)
        {
            context.Set(Token, "");
            context.Set(ExpiresAt, null);
        }
    }

    private string GenerateRandomToken(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var result = new char[length];

        for (int i = 0; i < length; i++)
            result[i] = chars[random.Next(chars.Length)];

        return new string(result);
    }

    private string GenerateNumericToken(int length)
    {
        var random = new Random();
        var result = new char[length];

        for (int i = 0; i < length; i++)
            result[i] = (char)('0' + random.Next(10));

        return new string(result);
    }
}
