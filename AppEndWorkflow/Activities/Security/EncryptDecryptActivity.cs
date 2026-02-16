using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

namespace AppEndWorkflow.Activities.Security;

/// <summary>
/// AES encryption and decryption.
/// Keys can be read from appsettings.json EncryptionKeys section.
/// </summary>
[Activity(
    Category = "Security",
    Description = "Encrypts or decrypts data using AES",
    DisplayName = "Encrypt/Decrypt"
)]
public class EncryptDecryptActivity : CodeActivity
{
    /// <summary>
    /// Data to encrypt or decrypt
    /// </summary>
    [Input(Description = "Data to encrypt or decrypt")]
    public Input<string> Data { get; set; } = default!;

    /// <summary>
    /// Encryption key (or key name from settings)
    /// </summary>
    [Input(Description = "Encryption key or key name")]
    public Input<string> Key { get; set; } = default!;

    /// <summary>
    /// Operation: "Encrypt" or "Decrypt"
    /// </summary>
    [Input(Description = "Operation: Encrypt or Decrypt")]
    public Input<string> Operation { get; set; } = new("Encrypt");

    /// <summary>
    /// Algorithm: "AES" (default: "AES")
    /// </summary>
    [Input(Description = "Algorithm")]
    public Input<string> Algorithm { get; set; } = new("AES");

    /// <summary>
    /// Encrypted/decrypted output (Base64 for encrypted)
    /// </summary>
    [Output(Description = "Encrypted/decrypted output")]
    public Output<string> Result { get; set; } = default!;

    /// <summary>
    /// Whether operation succeeded
    /// </summary>
    [Output(Description = "Whether operation succeeded")]
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
            var data = context.Get(Data);
            var key = context.Get(Key);
            var operation = context.Get(Operation);
            var algorithm = context.Get(Algorithm);

            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentException("'Data' is required");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("'Key' is required");

            if (string.IsNullOrWhiteSpace(operation))
                operation = "Encrypt";

            if (string.IsNullOrWhiteSpace(algorithm))
                algorithm = "AES";

            // TODO: Load key from configuration if it's a key name
            // For now, use the key value directly
            var keyBytes = ConvertKeyToBytes(key);

            // Perform operation
            var result = operation.ToLower() switch
            {
                "encrypt" => EncryptAes(data, keyBytes),
                "decrypt" => DecryptAes(data, keyBytes),
                _ => throw new NotSupportedException($"Operation '{operation}' is not supported")
            };

            context.Set(Result, result);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Result, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private byte[] ConvertKeyToBytes(string key)
    {
        // If key is hex, convert from hex; otherwise, use it as UTF8
        if (key.Length % 2 == 0 && IsHex(key))
        {
            return Convert.FromHexString(key);
        }

        // Use SHA256 of the key to get 32 bytes
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        return sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
    }

    private bool IsHex(string text)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(text, @"^[0-9A-Fa-f]+$");
    }

    private string EncryptAes(string plainText, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();

        // Write IV to stream
        ms.Write(aes.IV, 0, aes.IV.Length);

        // Encrypt data
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);
        sw.Write(plainText);
        sw.Flush();
        cs.FlushFinalBlock();

        // Return as Base64
        return Convert.ToBase64String(ms.ToArray());
    }

    private string DecryptAes(string cipherText, byte[] key)
    {
        var buffer = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Key = key;

        // Extract IV from buffer
        var iv = new byte[aes.IV.Length];
        Array.Copy(buffer, 0, iv, 0, iv.Length);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(buffer, iv.Length, buffer.Length - iv.Length);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}
