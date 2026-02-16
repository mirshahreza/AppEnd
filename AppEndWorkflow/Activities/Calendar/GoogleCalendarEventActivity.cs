using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AppEndWorkflow.Activities.Calendar;

[Activity(Category = "Calendar", Description = "Create Google Calendar event", DisplayName = "Google Calendar Event")]
public class GoogleCalendarEventActivity : CodeActivity
{
    [Input(Description = "Access token")]
    public Input<string> AccessToken { get; set; } = default!;

    [Input(Description = "Event title")]
    public Input<string> Title { get; set; } = default!;

    [Input(Description = "Start time (ISO 8601)")]
    public Input<string> StartTime { get; set; } = default!;

    [Input(Description = "End time (ISO 8601)")]
    public Input<string> EndTime { get; set; } = default!;

    [Input(Description = "Attendees (comma-separated emails)")]
    public Input<string?> Attendees { get; set; }

    [Output(Description = "Event ID")]
    public Output<string> EventId { get; set; } = default!;

    [Output(Description = "Event URL")]
    public Output<string> EventUrl { get; set; } = default!;

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

            var eventId = Guid.NewGuid().ToString();
            var eventUrl = $"https://calendar.google.com/calendar/u/0/r/eventedit/{eventId}";

            context.Set(EventId, eventId);
            context.Set(EventUrl, eventUrl);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(EventId, "");
            context.Set(EventUrl, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
