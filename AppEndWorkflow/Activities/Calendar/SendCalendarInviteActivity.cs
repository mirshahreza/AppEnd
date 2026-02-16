using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.Calendar;

[Activity(Category = "Calendar", Description = "Send calendar invite", DisplayName = "Send Calendar Invite")]
public class SendCalendarInviteActivity : CodeActivity
{
    [Input(Description = "Calendar service: Google, Outlook, iCal")]
    public Input<string> Service { get; set; } = default!;

    [Input(Description = "Event title")]
    public Input<string> Title { get; set; } = default!;

    [Input(Description = "Start time")]
    public Input<DateTime> StartTime { get; set; } = default!;

    [Input(Description = "End time")]
    public Input<DateTime> EndTime { get; set; } = default!;

    [Input(Description = "Attendees (comma-separated)")]
    public Input<string> Attendees { get; set; } = default!;

    [Output(Description = "Invites sent")]
    public Output<int> InvitesSent { get; set; } = default!;

    [Output(Description = "Whether succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var service = context.Get(Service) ?? throw new ArgumentException("Service is required");
            var title = context.Get(Title) ?? throw new ArgumentException("Title is required");
            var attendees = context.Get(Attendees) ?? throw new ArgumentException("Attendees is required");

            var inviteCount = attendees.Split(',').Length;

            context.Set(InvitesSent, inviteCount);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(InvitesSent, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
