using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace AppEndWorkflow.Activities.Email;

/// <summary>
/// Sends bulk emails with template support and rate limiting.
/// </summary>
[Activity(
    Category = "Email",
    Description = "Sends bulk emails with template support",
    DisplayName = "Send Bulk Email"
)]
public class SendBulkEmailActivity : CodeActivity
{
    /// <summary>
    /// JSON array of recipient objects with Email, Name, and Data properties
    /// </summary>
    [Input(Description = "JSON array of recipient objects [{Email, Name, Data}]")]
    public Input<string> Recipients { get; set; } = default!;

    /// <summary>
    /// Email template name or HTML content
    /// </summary>
    [Input(Description = "Email template name or HTML content")]
    public Input<string> TemplateName { get; set; } = default!;

    /// <summary>
    /// Email subject (supports placeholders like {{Name}})
    /// </summary>
    [Input(Description = "Email subject (supports placeholders)")]
    public Input<string> Subject { get; set; } = default!;

    /// <summary>
    /// Number of emails per batch
    /// </summary>
    [Input(Description = "Number of emails per batch (default: 100)")]
    public Input<int?> BatchSize { get; set; }

    /// <summary>
    /// Delay between batches in milliseconds
    /// </summary>
    [Input(Description = "Delay between batches in ms (default: 1000)")]
    public Input<int?> DelayMs { get; set; }

    /// <summary>
    /// Number of emails sent successfully
    /// </summary>
    [Output(Description = "Number of emails sent successfully")]
    public Output<int> SentCount { get; set; } = default!;

    /// <summary>
    /// Number of failed sends
    /// </summary>
    [Output(Description = "Number of failed sends")]
    public Output<int> FailedCount { get; set; } = default!;

    /// <summary>
    /// Whether operation completed
    /// </summary>
    [Output(Description = "Whether operation completed")]
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
            var recipients = context.Get(Recipients) ?? throw new ArgumentException("Recipients is required");
            var templateName = context.Get(TemplateName) ?? throw new ArgumentException("TemplateName is required");
            var subject = context.Get(Subject) ?? throw new ArgumentException("Subject is required");
            var batchSize = context.Get(BatchSize) ?? 100;
            var delayMs = context.Get(DelayMs) ?? 1000;

            var configuration = context.GetService<IConfiguration>();
            var smtpConfig = configuration?.GetSection("Smtp");
            
            if (smtpConfig == null || !smtpConfig.Exists())
                throw new InvalidOperationException("SMTP configuration not found");

            var smtpHost = smtpConfig["Host"] ?? throw new InvalidOperationException("SMTP Host not configured");
            var smtpPort = int.Parse(smtpConfig["Port"] ?? "587");
            var smtpUsername = smtpConfig["Username"] ?? throw new InvalidOperationException("SMTP Username not configured");
            var smtpPassword = smtpConfig["Password"] ?? throw new InvalidOperationException("SMTP Password not configured");
            var enableSsl = bool.Parse(smtpConfig["EnableSsl"] ?? "true");
            var fromAddress = smtpConfig["FromAddress"] ?? smtpUsername;

            // Parse recipients
            var recipientList = new List<Dictionary<string, object>>();
            try
            {
                using var doc = JsonDocument.Parse(recipients);
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var elem in doc.RootElement.EnumerateArray())
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (var prop in elem.EnumerateObject())
                        {
                            dict[prop.Name] = prop.Value.GetRawText();
                        }
                        recipientList.Add(dict);
                    }
                }
            }
            catch
            {
                throw new ArgumentException("Recipients must be valid JSON array");
            }

            int sentCount = 0, failedCount = 0;

            // Process in batches
            for (int i = 0; i < recipientList.Count; i += batchSize)
            {
                var batch = recipientList.Skip(i).Take(batchSize);

                foreach (var recipient in batch)
                {
                    try
                    {
                        var email = recipient.ContainsKey("Email") ? recipient["Email"].ToString() : null;
                        var name = recipient.ContainsKey("Name") ? recipient["Name"].ToString() : null;

                        if (string.IsNullOrWhiteSpace(email))
                        {
                            failedCount++;
                            continue;
                        }

                        var emailSubject = ReplaceTemplate(subject, name ?? "", recipient);

                        using var mailMessage = new MailMessage();
                        mailMessage.From = new MailAddress(fromAddress);
                        mailMessage.Subject = emailSubject;
                        mailMessage.Body = templateName; // Simplified: template as body
                        mailMessage.IsBodyHtml = true;
                        mailMessage.To.Add(email);

                        using var smtpClient = new SmtpClient(smtpHost, smtpPort);
                        smtpClient.EnableSsl = enableSsl;
                        smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                        smtpClient.Send(mailMessage);

                        sentCount++;
                    }
                    catch
                    {
                        failedCount++;
                    }
                }

                // Delay between batches
                if (i + batchSize < recipientList.Count)
                    System.Threading.Thread.Sleep(delayMs);
            }

            context.Set(SentCount, sentCount);
            context.Set(FailedCount, failedCount);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(SentCount, 0);
            context.Set(FailedCount, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private static string ReplaceTemplate(string template, string name, Dictionary<string, object> data)
    {
        var result = template.Replace("{{Name}}", name);
        foreach (var kvp in data)
        {
            result = result.Replace($"{{{{{kvp.Key}}}}}", kvp.Value?.ToString() ?? "");
        }
        return result;
    }
}
