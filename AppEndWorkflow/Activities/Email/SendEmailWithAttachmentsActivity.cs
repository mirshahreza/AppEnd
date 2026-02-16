using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace AppEndWorkflow.Activities.Email;

/// <summary>
/// Sends email with file attachments.
/// </summary>
[Activity(
    Category = "Email",
    Description = "Sends email with file attachments",
    DisplayName = "Send Email With Attachments"
)]
public class SendEmailWithAttachmentsActivity : CodeActivity
{
    [Input(Description = "Recipient email address(es)")]
    public Input<string> To { get; set; } = default!;

    [Input(Description = "Email subject")]
    public Input<string> Subject { get; set; } = default!;

    [Input(Description = "Email body")]
    public Input<string> Body { get; set; } = default!;

    [Input(Description = "JSON array of attachments [{FilePath, ContentType?, FileName?}]")]
    public Input<string> Attachments { get; set; } = default!;

    [Output(Description = "Whether email was sent")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Number of attachments sent")]
    public Output<int> AttachmentCount { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var to = context.Get(To) ?? throw new ArgumentException("To is required");
            var subject = context.Get(Subject) ?? throw new ArgumentException("Subject is required");
            var body = context.Get(Body) ?? throw new ArgumentException("Body is required");
            var attachmentsJson = context.Get(Attachments) ?? "[]";

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

            using var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(fromAddress);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            // Add recipients
            foreach (var address in to.Split(','))
            {
                var trimmedAddress = address.Trim();
                if (!string.IsNullOrWhiteSpace(trimmedAddress))
                    mailMessage.To.Add(trimmedAddress);
            }

            // Parse and add attachments
            int attachmentCount = 0;
            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(attachmentsJson);
                if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    foreach (var elem in doc.RootElement.EnumerateArray())
                    {
                        if (elem.TryGetProperty("FilePath", out var filePath))
                        {
                            var path = filePath.GetString();
                            if (File.Exists(path))
                            {
                                var attachment = new Attachment(path);

                                if (elem.TryGetProperty("FileName", out var fileName))
                                    attachment.Name = fileName.GetString() ?? Path.GetFileName(path);

                                if (elem.TryGetProperty("ContentType", out var contentType))
                                    attachment.ContentType.MediaType = contentType.GetString() ?? "application/octet-stream";

                                mailMessage.Attachments.Add(attachment);
                                attachmentCount++;
                            }
                        }
                    }
                }
            }
            catch
            {
                // Continue without attachments if parsing fails
            }

            // Send email
            using var smtpClient = new SmtpClient(smtpHost, smtpPort);
            smtpClient.EnableSsl = enableSsl;
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            smtpClient.Send(mailMessage);

            context.Set(Success, true);
            context.Set(AttachmentCount, attachmentCount);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Success, false);
            context.Set(AttachmentCount, 0);
            context.Set(Error, ex.Message);
        }
    }
}
