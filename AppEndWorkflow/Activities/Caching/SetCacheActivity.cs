using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using Microsoft.Extensions.Caching.Memory;

namespace AppEndWorkflow.Activities.Caching;

/// <summary>
/// Stores value in memory cache with optional expiration.
/// </summary>
[Activity(
    Category = "Cache",
    Description = "Stores value in cache",
    DisplayName = "Set Cache"
)]
public class SetCacheActivity : CodeActivity
{
    /// <summary>
    /// Cache key
    /// </summary>
    [Input(Description = "Cache key")]
    public Input<string> Key { get; set; } = default!;

    /// <summary>
    /// Value to cache (JSON or string)
    /// </summary>
    [Input(Description = "Value to cache")]
    public Input<string> Value { get; set; } = default!;

    /// <summary>
    /// Expiration time in minutes (optional)
    /// </summary>
    [Input(Description = "Expiration in minutes")]
    public Input<int?> ExpirationMinutes { get; set; }

    /// <summary>
    /// Whether value was cached
    /// </summary>
    [Output(Description = "Whether value was cached")]
    public Output<bool> Success { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var key = context.Get(Key);
            var value = context.Get(Value);
            var expirationMinutes = context.Get(ExpirationMinutes);

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("'Key' is required");

            if (value == null)
                throw new ArgumentException("'Value' is required");

            // Get memory cache
            var cache = context.GetService<IMemoryCache>();
            if (cache == null)
                throw new InvalidOperationException("IMemoryCache service not available");

            // Set cache with optional expiration
            if (expirationMinutes.HasValue && expirationMinutes > 0)
            {
                var options = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(expirationMinutes.Value));
                cache.Set(key, value, options);
            }
            else
            {
                cache.Set(key, value);
            }

            context.Set(Success, true);
        }
        catch (Exception ex)
        {
            context.Set(Success, false);
        }
    }
}
