using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AppEndWorkflow.Activities.Text;

/// <summary>
/// Performs regex pattern matching on input text.
/// </summary>
[Activity(
    Category = "Text",
    Description = "Regex pattern matching",
    DisplayName = "Regex Match"
)]
public class RegexMatchActivity : CodeActivity
{
    /// <summary>
    /// String to search
    /// </summary>
    [Input(Description = "String to search")]
    public Input<string> Input { get; set; } = default!;

    /// <summary>
    /// Regular expression pattern
    /// </summary>
    [Input(Description = "Regular expression pattern")]
    public Input<string> Pattern { get; set; } = default!;

    /// <summary>
    /// Regex options: "IgnoreCase", "Multiline", etc.
    /// </summary>
    [Input(Description = "Regex options (optional)")]
    public Input<string?> Options { get; set; }

    /// <summary>
    /// Whether the pattern matched
    /// </summary>
    [Output(Description = "Whether the pattern matched")]
    public Output<bool> IsMatch { get; set; } = default!;

    /// <summary>
    /// JSON array of matched groups
    /// </summary>
    [Output(Description = "JSON array of matched groups")]
    public Output<string> Matches { get; set; } = default!;

    /// <summary>
    /// Number of matches
    /// </summary>
    [Output(Description = "Number of matches")]
    public Output<int> MatchCount { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var input = context.Get(Input);
            var pattern = context.Get(Pattern);
            var optionsStr = context.Get(Options);

            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("'Input' is required");

            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentException("'Pattern' is required");

            // Parse regex options
            var options = ParseRegexOptions(optionsStr);

            // Perform matching
            var regex = new Regex(pattern, options);
            var matches = regex.Matches(input);

            // Extract match data
            var matchList = new List<Dictionary<string, string>>();
            foreach (Match match in matches)
            {
                var matchData = new Dictionary<string, string>
                {
                    { "Value", match.Value },
                    { "Index", match.Index.ToString() },
                    { "Length", match.Length.ToString() }
                };

                // Add groups
                for (int i = 0; i < match.Groups.Count; i++)
                {
                    matchData[$"Group{i}"] = match.Groups[i].Value;
                }

                matchList.Add(matchData);
            }

            context.Set(IsMatch, matches.Count > 0);
            context.Set(Matches, JsonSerializer.Serialize(matchList));
            context.Set(MatchCount, matches.Count);
        }
        catch (Exception ex)
        {
            context.Set(IsMatch, false);
            context.Set(Matches, "[]");
            context.Set(MatchCount, 0);
        }
    }

    private RegexOptions ParseRegexOptions(string? optionsStr)
    {
        if (string.IsNullOrWhiteSpace(optionsStr))
            return RegexOptions.None;

        var options = RegexOptions.None;
        var optionsList = optionsStr.Split(',');

        foreach (var option in optionsList)
        {
            switch (option.Trim().ToLower())
            {
                case "ignorecase":
                    options |= RegexOptions.IgnoreCase;
                    break;
                case "multiline":
                    options |= RegexOptions.Multiline;
                    break;
                case "singleline":
                    options |= RegexOptions.Singleline;
                    break;
                case "ignorewhitespace":
                    options |= RegexOptions.IgnorePatternWhitespace;
                    break;
            }
        }

        return options;
    }
}
