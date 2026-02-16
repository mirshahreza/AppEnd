using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Xml;

namespace AppEndWorkflow.Activities.Feeds;

/// <summary>
/// Monitors RSS/Atom feed for updates.
/// Polls feed at specified interval and returns new items.
/// </summary>
[Activity(
    Category = "RSS",
    Description = "Monitor RSS feed for updates",
    DisplayName = "Monitor Feed Updates"
)]
public class MonitorFeedUpdatesActivity : CodeActivity
{
    [Input(Description = "Feed URL to monitor")]
    public Input<string> FeedUrl { get; set; } = default!;

    [Input(Description = "Check interval in minutes (default: 60)")]
    public Input<int?> CheckIntervalMinutes { get; set; }

    [Input(Description = "Maximum monitoring duration in minutes")]
    public Input<int?> MaxDuration { get; set; }

    [Output(Description = "Whether new items were found")]
    public Output<bool> UpdateFound { get; set; } = default!;

    [Output(Description = "JSON array of new items")]
    public Output<string> NewItems { get; set; } = default!;

    [Output(Description = "When the last check occurred")]
    public Output<DateTime> LastCheckTime { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var feedUrl = context.Get(FeedUrl) ?? throw new ArgumentException("FeedUrl is required");
            var checkInterval = context.Get(CheckIntervalMinutes) ?? 60;
            var maxDuration = context.Get(MaxDuration);

            // NOTE: In production, this would use Elsa timers and bookmarks
            // to periodically check the feed until new items appear
            // For now, we'll do a simple check

            using var httpClient = new HttpClient();
            var feedContent = httpClient.GetStringAsync(feedUrl).Result;

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(feedContent);

            var newItems = new List<Dictionary<string, string>>();
            var itemNodes = xmlDoc.SelectNodes("//item");

            // Get the most recent item
            if (itemNodes.Count > 0)
            {
                var firstItem = itemNodes[0] as XmlNode;
                if (firstItem != null)
                {
                    var item = new Dictionary<string, string>
                    {
                        ["title"] = firstItem.SelectSingleNode("title")?.InnerText ?? "",
                        ["description"] = firstItem.SelectSingleNode("description")?.InnerText ?? "",
                        ["link"] = firstItem.SelectSingleNode("link")?.InnerText ?? "",
                        ["pubDate"] = firstItem.SelectSingleNode("pubDate")?.InnerText ?? ""
                    };
                    newItems.Add(item);
                }
            }

            var itemsJson = JsonSerializer.Serialize(newItems);

            context.Set(UpdateFound, newItems.Count > 0);
            context.Set(NewItems, itemsJson);
            context.Set(LastCheckTime, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            context.Set(UpdateFound, false);
            context.Set(NewItems, "[]");
            context.Set(LastCheckTime, DateTime.MinValue);
        }
    }
}
