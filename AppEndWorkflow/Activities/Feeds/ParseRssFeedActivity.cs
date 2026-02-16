using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Xml;

namespace AppEndWorkflow.Activities.Feeds;

/// <summary>
/// Parses RSS/Atom feed and extracts items.
/// </summary>
[Activity(
    Category = "RSS",
    Description = "Parse RSS/Atom feed",
    DisplayName = "Parse RSS Feed"
)]
public class ParseRssFeedActivity : CodeActivity
{
    [Input(Description = "RSS/Atom feed URL")]
    public Input<string> FeedUrl { get; set; } = default!;

    [Input(Description = "Maximum items to parse (default: 50)")]
    public Input<int?> ItemLimit { get; set; }

    [Input(Description = "Feed fetch timeout in seconds (default: 30)")]
    public Input<int?> TimeoutSeconds { get; set; }

    [Output(Description = "JSON array of parsed feed items")]
    public Output<string> Items { get; set; } = default!;

    [Output(Description = "Number of items parsed")]
    public Output<int> ItemCount { get; set; } = default!;

    [Output(Description = "Feed title")]
    public Output<string> Title { get; set; } = default!;

    [Output(Description = "Whether parsing succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var feedUrl = context.Get(FeedUrl) ?? throw new ArgumentException("FeedUrl is required");
            var itemLimit = context.Get(ItemLimit) ?? 50;
            var timeoutSeconds = context.Get(TimeoutSeconds) ?? 30;

            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

            var feedContent = httpClient.GetStringAsync(feedUrl).Result;

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(feedContent);

            var items = new List<Dictionary<string, string>>();
            var feedTitle = "Unknown Feed";

            // Try to parse as RSS
            var channelNode = xmlDoc.SelectSingleNode("//channel");
            if (channelNode != null)
            {
                feedTitle = channelNode.SelectSingleNode("title")?.InnerText ?? "Unknown";
                var itemNodes = xmlDoc.SelectNodes("//item").Cast<XmlNode>().Take(itemLimit);

                foreach (var itemNode in itemNodes)
                {
                    var item = new Dictionary<string, string>
                    {
                        ["title"] = itemNode.SelectSingleNode("title")?.InnerText ?? "",
                        ["description"] = itemNode.SelectSingleNode("description")?.InnerText ?? "",
                        ["link"] = itemNode.SelectSingleNode("link")?.InnerText ?? "",
                        ["pubDate"] = itemNode.SelectSingleNode("pubDate")?.InnerText ?? "",
                        ["guid"] = itemNode.SelectSingleNode("guid")?.InnerText ?? ""
                    };
                    items.Add(item);
                }
            }
            else
            {
                // Try Atom format
                var entryNodes = xmlDoc.SelectNodes("//atom:entry", CreateNamespaceManager(xmlDoc)).Cast<XmlNode>().Take(itemLimit);
                feedTitle = xmlDoc.SelectSingleNode("//atom:feed/atom:title", CreateNamespaceManager(xmlDoc))?.InnerText ?? "Unknown";

                foreach (var entryNode in entryNodes)
                {
                    var nsmgr = CreateNamespaceManager(xmlDoc);
                    var item = new Dictionary<string, string>
                    {
                        ["title"] = entryNode.SelectSingleNode("atom:title", nsmgr)?.InnerText ?? "",
                        ["summary"] = entryNode.SelectSingleNode("atom:summary", nsmgr)?.InnerText ?? "",
                        ["link"] = entryNode.SelectSingleNode("atom:link/@href", nsmgr)?.Value ?? "",
                        ["published"] = entryNode.SelectSingleNode("atom:published", nsmgr)?.InnerText ?? ""
                    };
                    items.Add(item);
                }
            }

            var itemsJson = JsonSerializer.Serialize(items);

            context.Set(Items, itemsJson);
            context.Set(ItemCount, items.Count);
            context.Set(Title, feedTitle);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Items, "[]");
            context.Set(ItemCount, 0);
            context.Set(Title, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private XmlNamespaceManager CreateNamespaceManager(XmlDocument doc)
    {
        var nsmgr = new XmlNamespaceManager(doc.NameTable);
        nsmgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
        return nsmgr;
    }
}
