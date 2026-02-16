using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.Calendar;

[Activity(Category = "Calendar", Description = "Schedule meeting with Calendly", DisplayName = "Calendly Booking")]
public class CalendlyBookingActivity : CodeActivity
{
    [Input(Description = "Personal access token")]
    public Input<string> AccessToken { get; set; } = default!;

    [Input(Description = "Calendly username")]
    public Input<string> Username { get; set; } = default!;

    [Input(Description = "Event type slug")]
    public Input<string> EventTypeSlug { get; set; } = default!;

    [Input(Description = "Invitee name")]
    public Input<string> InviteeName { get; set; } = default!;

    [Input(Description = "Invitee email")]
    public Input<string> InviteeEmail { get; set; } = default!;

    [Output(Description = "Booking ID")]
    public Output<string> BookingId { get; set; } = default!;

    [Output(Description = "Booking URL")]
    public Output<string> BookingUrl { get; set; } = default!;

    [Output(Description = "Whether succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var accessToken = context.Get(AccessToken) ?? throw new ArgumentException("AccessToken is required");
            var username = context.Get(Username) ?? throw new ArgumentException("Username is required");
            var eventTypeSlug = context.Get(EventTypeSlug) ?? throw new ArgumentException("EventTypeSlug is required");

            var bookingId = Guid.NewGuid().ToString();
            var bookingUrl = $"https://{username}.calendly.com/{eventTypeSlug}";

            context.Set(BookingId, bookingId);
            context.Set(BookingUrl, bookingUrl);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(BookingId, "");
            context.Set(BookingUrl, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
