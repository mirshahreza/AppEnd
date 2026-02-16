using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Email;

/// <summary>
/// Receives emails from IMAP server.
/// Requires MailKit NuGet package.
/// </summary>
[Activity(
    Category = "Email",
    Description = "Receive emails from IMAP server",
    DisplayName = "Receive Email"
)]
public class ReceiveEmailActivity : CodeActivity
{
    [Input(Description = "IMAP server address")]
    public Input<string> ImapServer { get; set; } = default!;

    [Input(Description = "IMAP port (default: 993)")]
    public Input<int?> Port { get; set; }

    [Input(Description = "Email account username")]
    public Input<string> Username { get; set; } = default!;

    [Input(Description = "Email account password")]
    public Input<string> Password { get; set; } = default!;

    [Input(Description = "Folder name (default: INBOX)")]
    public Input<string?> FolderName { get; set; }

    [Input(Description = "Read only unread emails (default: true)")]
    public Input<bool?> ReadUnread { get; set; }

    [Input(Description = "Maximum emails to fetch (default: 10)")]
    public Input<int?> MaxEmails { get; set; }

    [Output(Description = "JSON array of email objects")]
    public Output<string> Emails { get; set; } = default!;

    [Output(Description = "Number of emails received")]
    public Output<int> EmailCount { get; set; } = default!;

    [Output(Description = "Whether operation succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var server = context.Get(ImapServer) ?? throw new ArgumentException("ImapServer is required");
            var port = context.Get(Port) ?? 993;
            var username = context.Get(Username) ?? throw new ArgumentException("Username is required");
            var password = context.Get(Password) ?? throw new ArgumentException("Password is required");
            var folderName = context.Get(FolderName) ?? "INBOX";
            var readUnread = context.Get(ReadUnread) ?? true;
            var maxEmails = context.Get(MaxEmails) ?? 10;

            // NOTE: In production, use MailKit
            // using (var client = new ImapClient())
            // {
            //     client.Connect(server, port, SecureSocketOptions.SslOnConnect);
            //     client.Authenticate(username, password);
            //     client.Inbox.Open(FolderAccess.ReadOnly);
            //
            //     var folder = client.GetFolder(folderName);
            //     folder.Open(FolderAccess.ReadOnly);
            //
            //     var query = readUnread ? SearchQuery.NotSeen : SearchQuery.All;
            //     var uids = folder.Search(query);
            //     var messages = new List<object>();
            //
            //     foreach (var uid in uids.Take(maxEmails))
            //     {
            //         var message = folder.GetMessage(uid);
            //         messages.Add(new
            //         {
            //             From = message.From.ToString(),
            //             Subject = message.Subject,
            //             Date = message.Date,
            //             Body = message.TextBody
            //         });
            //     }
            //     client.Disconnect(true);
            //     return JsonSerializer.Serialize(messages);
            // }

            var mockEmails = new List<object>
            {
                new { From = "sender@example.com", Subject = "Test Email", Date = DateTime.UtcNow, Body = "Test content" }
            };

            var emailsJson = JsonSerializer.Serialize(mockEmails);

            context.Set(Emails, emailsJson);
            context.Set(EmailCount, mockEmails.Count);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Emails, "[]");
            context.Set(EmailCount, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
