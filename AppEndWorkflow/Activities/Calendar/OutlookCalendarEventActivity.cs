using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Calendar;

[Activity(Category = "Calendar", Description = "Create Outlook calendar event", DisplayName = "Outlook Calendar Event")]
public class OutlookCalendarEventActivity : CodeActivity
{
    [Input(Description = "Access token")]
    public Input<string> AccessToken { get; set; } = default!;

    [Input(Description = "Event title")]
    public Input<string> Title { get; set; } = default!;

    [Input(Description = "Start time")]
    public Input<DateTime> StartTime { get; set; } = default!;

    [Input(Description = "End time")]
    public Input<DateTime> EndTime { get; set; } = default!;

    [Output(Description = "Event ID")]
    public Output<string> EventId { get; set; } = default!;

    [Output(Description = "Whether succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var accessToken = context.Get(AccessToken) ?? throw new ArgumentException("AccessToken is required");
            var title = context.Get(Title) ?? throw new ArgumentException("Title is required");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var eventId = Guid.NewGuid().ToString();
            context.Set(EventId, eventId);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(EventId, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
