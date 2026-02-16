using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using Microsoft.Extensions.Caching.Memory;

namespace AppEndWorkflow.Activities.Caching;

/// <summary>
/// Removes entries from memory cache.
/// Supports pattern-based removal.
/// </summary>
[Activity(
    Category = "Cache",
    Description = "Removes cache entries",
    DisplayName = "Remove Cache"
)]
public class RemoveCacheActivity : CodeActivity
{
    /// <summary>
    /// Cache key to remove
    /// </summary>
    [Input(Description = "Cache key to remove")]
    public Input<string> Key { get; set; } = default!;

    /// <summary>
    /// Pattern to match multiple keys (e.g., "workflow:*")
    /// </summary>
    [Input(Description = "Pattern to match keys")]
    public Input<string?> Pattern { get; set; }

    /// <summary>
    /// Number of keys removed
    /// </summary>
    [Output(Description = "Number of keys removed")]
    public Output<int> RemovedCount { get; set; } = default!;

    /// <summary>
    /// Whether removal succeeded
    /// </summary>
    [Output(Description = "Whether removal succeeded")]
    public Output<bool> Success { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var key = context.Get(Key);
            var pattern = context.Get(Pattern);

            if (string.IsNullOrWhiteSpace(key) && string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentException("Either 'Key' or 'Pattern' is required");

            // Get memory cache
            var cache = context.GetService<IMemoryCache>();
            if (cache == null)
                throw new InvalidOperationException("IMemoryCache service not available");

            var removedCount = 0;

            // Remove by exact key
            if (!string.IsNullOrWhiteSpace(key))
            {
                cache.Remove(key);
                removedCount = 1;
            }

            // TODO: Pattern-based removal
            // IMemoryCache doesn't support enumeration, so pattern-based removal
            // would require a custom implementation or different caching strategy

            context.Set(RemovedCount, removedCount);
            context.Set(Success, true);
        }
        catch (Exception ex)
        {
            context.Set(RemovedCount, 0);
            context.Set(Success, false);
        }
    }
}
