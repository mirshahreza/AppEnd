using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

namespace AppEndWorkflow.Activities.Security;

/// <summary>
/// Hashes data using various algorithms (SHA256, SHA512, MD5, SHA1).
/// </summary>
[Activity(
    Category = "Security",
    Description = "Hashes data",
    DisplayName = "Hash Data"
)]
public class HashDataActivity : CodeActivity
{
    /// <summary>
    /// Data to hash
    /// </summary>
    [Input(Description = "Data to hash")]
    public Input<string> Data { get; set; } = default!;

    /// <summary>
    /// Algorithm: "SHA256", "SHA512", "MD5", "SHA1" (default: "SHA256")
    /// </summary>
    [Input(Description = "Algorithm: SHA256, SHA512, MD5, SHA1")]
    public Input<string> Algorithm { get; set; } = new("SHA256");

    /// <summary>
    /// Output format: "Hex" or "Base64" (default: "Hex")
    /// </summary>
    [Input(Description = "Output format: Hex or Base64")]
    public Input<string> OutputFormat { get; set; } = new("Hex");

    /// <summary>
    /// Hash result
    /// </summary>
    [Output(Description = "Hash result")]
    public Output<string> Hash { get; set; } = default!;

    /// <summary>
    /// Algorithm used
    /// </summary>
    [Output(Description = "Algorithm used")]
    public Output<string> Algorithm_Out { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var data = context.Get(Data);
            var algorithm = context.Get(Algorithm);
            var outputFormat = context.Get(OutputFormat);

            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentException("'Data' is required");

            if (string.IsNullOrWhiteSpace(algorithm))
                algorithm = "SHA256";

            if (string.IsNullOrWhiteSpace(outputFormat))
                outputFormat = "Hex";

            // Compute hash
            var hash = ComputeHash(data, algorithm, outputFormat);

            context.Set(Hash, hash);
            context.Set(Algorithm_Out, algorithm);
        }
        catch (Exception ex)
        {
            context.Set(Hash, "");
            context.Set(Algorithm_Out, "");
        }
    }

    private string ComputeHash(string data, string algorithm, string outputFormat)
    {
        var dataBytes = Encoding.UTF8.GetBytes(data);
        byte[] hashBytes;

        switch (algorithm.ToUpper())
        {
            case "SHA256":
                using (var sha256 = SHA256.Create())
                    hashBytes = sha256.ComputeHash(dataBytes);
                break;

            case "SHA512":
                using (var sha512 = SHA512.Create())
                    hashBytes = sha512.ComputeHash(dataBytes);
                break;

            case "MD5":
                using (var md5 = MD5.Create())
                    hashBytes = md5.ComputeHash(dataBytes);
                break;

            case "SHA1":
                using (var sha1 = SHA1.Create())
                    hashBytes = sha1.ComputeHash(dataBytes);
                break;

            default:
                throw new NotSupportedException($"Algorithm '{algorithm}' is not supported");
        }

        // Format output
        return outputFormat.ToLower() switch
        {
            "base64" => Convert.ToBase64String(hashBytes),
            "hex" => BitConverter.ToString(hashBytes).Replace("-", "").ToLower(),
            _ => BitConverter.ToString(hashBytes).Replace("-", "").ToLower()
        };
    }
}
