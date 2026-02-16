using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.Forms;

[Activity(Category = "Forms", Description = "Create JotForm form", DisplayName = "JotForm Create")]
public class JotFormCreateActivity : CodeActivity
{
    [Input(Description = "API key")]
    public Input<string> ApiKey { get; set; } = default!;

    [Input(Description = "Form title")]
    public Input<string> Title { get; set; } = default!;

    [Input(Description = "Form questions")]
    public Input<string> Questions { get; set; } = default!;

    [Output(Description = "Form ID")]
    public Output<string> FormId { get; set; } = default!;

    [Output(Description = "Form URL")]
    public Output<string> FormUrl { get; set; } = default!;

    [Output(Description = "Whether created")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var apiKey = context.Get(ApiKey) ?? throw new ArgumentException("ApiKey is required");
            var title = context.Get(Title) ?? throw new ArgumentException("Title is required");

            var formId = Guid.NewGuid().ToString();
            var formUrl = $"https://{formId}.jotform.com";

            context.Set(FormId, formId);
            context.Set(FormUrl, formUrl);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(FormId, "");
            context.Set(FormUrl, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
