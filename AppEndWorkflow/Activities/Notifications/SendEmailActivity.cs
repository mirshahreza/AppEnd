using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace AppEndWorkflow.Activities.Notifications;

/// <summary>
/// Sends an email via SMTP.
/// SMTP settings are read from appsettings.json under the "Smtp" section.
/// </summary>
[Activity(
    Category = "Notifications",
    Description = "Sends an email via SMTP",
    DisplayName = "Send Email"
)]
public class SendEmailActivity : CodeActivity
{
    /// <summary>
    /// Recipient email address(es), comma-separated
    /// </summary>
    [Input(Description = "Recipient email address(es), comma-separated")]
    public Input<string> To { get; set; } = default!;

    /// <summary>
    /// Email subject
    /// </summary>
    [Input(Description = "Email subject")]
    public Input<string> Subject { get; set; } = default!;

    /// <summary>
    /// Email body (HTML supported)
    /// </summary>
    [Input(Description = "Email body (HTML supported)")]
    public Input<string> Body { get; set; } = default!;

    /// <summary>
    /// CC recipients (optional)
    /// </summary>
    [Input(Description = "CC recipients (optional)")]
    public Input<string?> Cc { get; set; }

    /// <summary>
    /// BCC recipients (optional)
    /// </summary>
    [Input(Description = "BCC recipients (optional)")]
    public Input<string?> Bcc { get; set; }

    /// <summary>
    /// Whether the email was sent successfully
    /// </summary>
    [Output(Description = "Whether the email was sent successfully")]
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
            var to = context.Get(To);
            var subject = context.Get(Subject);
            var body = context.Get(Body);
            var cc = context.Get(Cc);
            var bcc = context.Get(Bcc);

            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("'To' address is required");

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("'Subject' is required");

            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("'Body' is required");

            // Get SMTP settings from configuration
            var configuration = context.GetService<IConfiguration>();
            var smtpConfig = configuration?.GetSection("Smtp");

            if (smtpConfig == null || !smtpConfig.Exists())
                throw new InvalidOperationException("SMTP configuration not found in appsettings.json");

            var smtpHost = smtpConfig["Host"] ?? throw new InvalidOperationException("SMTP Host not configured");
            var smtpPort = int.Parse(smtpConfig["Port"] ?? "587");
            var smtpUsername = smtpConfig["Username"] ?? throw new InvalidOperationException("SMTP Username not configured");
            var smtpPassword = smtpConfig["Password"] ?? throw new InvalidOperationException("SMTP Password not configured");
            var enableSsl = bool.Parse(smtpConfig["EnableSsl"] ?? "true");
            var fromAddress = smtpConfig["FromAddress"] ?? smtpUsername;

            // Create mail message
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

            // Add CC if provided
            if (!string.IsNullOrWhiteSpace(cc))
            {
                foreach (var address in cc.Split(','))
                {
                    var trimmedAddress = address.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmedAddress))
                        mailMessage.CC.Add(trimmedAddress);
                }
            }

            // Add BCC if provided
            if (!string.IsNullOrWhiteSpace(bcc))
            {
                foreach (var address in bcc.Split(','))
                {
                    var trimmedAddress = address.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmedAddress))
                        mailMessage.Bcc.Add(trimmedAddress);
                }
            }

            // Send email via SMTP
            using var smtpClient = new SmtpClient(smtpHost, smtpPort);
            smtpClient.EnableSsl = enableSsl;
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            smtpClient.Send(mailMessage);

            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
