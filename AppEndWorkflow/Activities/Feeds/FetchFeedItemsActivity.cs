using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Xml;

namespace AppEndWorkflow.Activities.Feeds;

/// <summary>
/// Fetches recent items from RSS/Atom feed with filtering.
/// </summary>
[Activity(
    Category = "RSS",
    Description = "Fetch RSS feed items",
    DisplayName = "Fetch Feed Items"
)]
public class FetchFeedItemsActivity : CodeActivity
{
    [Input(Description = "Feed URL")]
    public Input<string> FeedUrl { get; set; } = default!;

    [Input(Description = "Only items published after this date")]
    public Input<DateTime?> SinceDateUtc { get; set; }

    [Input(Description = "Filter items by keyword")]
    public Input<string?> Keyword { get; set; }

    [Output(Description = "JSON array of items")]
    public Output<string> Items { get; set; } = default!;

    [Output(Description = "Number of new items")]
    public Output<int> NewItemCount { get; set; } = default!;

    [Output(Description = "Whether fetch succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var feedUrl = context.Get(FeedUrl) ?? throw new ArgumentException("FeedUrl is required");
            var sinceDate = context.Get(SinceDateUtc);
            var keyword = context.Get(Keyword);

            using var httpClient = new HttpClient();
            var feedContent = httpClient.GetStringAsync(feedUrl).Result;

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(feedContent);

            var items = new List<Dictionary<string, string>>();

            // Parse RSS items
            var itemNodes = xmlDoc.SelectNodes("//item");
            foreach (var itemNode in itemNodes.Cast<XmlNode>())
            {
                var title = itemNode.SelectSingleNode("title")?.InnerText ?? "";
                var description = itemNode.SelectSingleNode("description")?.InnerText ?? "";
                var pubDateStr = itemNode.SelectSingleNode("pubDate")?.InnerText ?? "";

                // Apply filters
                if (!string.IsNullOrWhiteSpace(keyword) && 
                    !title.Contains(keyword, StringComparison.OrdinalIgnoreCase) &&
                    !description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (sinceDate.HasValue && DateTime.TryParse(pubDateStr, out var pubDate))
                {
                    if (pubDate < sinceDate.Value)
                        continue;
                }

                var item = new Dictionary<string, string>
                {
                    ["title"] = title,
                    ["description"] = description,
                    ["link"] = itemNode.SelectSingleNode("link")?.InnerText ?? "",
                    ["pubDate"] = pubDateStr
                };
                items.Add(item);
            }

            var itemsJson = JsonSerializer.Serialize(items);

            context.Set(Items, itemsJson);
            context.Set(NewItemCount, items.Count);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Items, "[]");
            context.Set(NewItemCount, 0);
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}
