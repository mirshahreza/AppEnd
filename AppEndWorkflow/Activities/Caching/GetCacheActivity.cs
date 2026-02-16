using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using Microsoft.Extensions.Caching.Memory;

namespace AppEndWorkflow.Activities.Caching;

/// <summary>
/// Retrieves value from memory cache.
/// </summary>
[Activity(
    Category = "Cache",
    Description = "Retrieves value from cache",
    DisplayName = "Get Cache"
)]
public class GetCacheActivity : CodeActivity
{
    /// <summary>
    /// Cache key
    /// </summary>
    [Input(Description = "Cache key")]
    public Input<string> Key { get; set; } = default!;

    /// <summary>
    /// Value to return if key not found
    /// </summary>
    [Input(Description = "Default value if not found")]
    public Input<string?> DefaultValue { get; set; }

    /// <summary>
    /// Cached value
    /// </summary>
    [Output(Description = "Cached value")]
    public Output<string?> Value { get; set; }

    /// <summary>
    /// Whether the key was found in cache
    /// </summary>
    [Output(Description = "Whether the key was found")]
    public Output<bool> Found { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var key = context.Get(Key);
            var defaultValue = context.Get(DefaultValue);

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("'Key' is required");

            // Get memory cache
            var cache = context.GetService<IMemoryCache>();
            if (cache == null)
                throw new InvalidOperationException("IMemoryCache service not available");

            // Try to get value
            if (cache.TryGetValue(key, out var cachedValue))
            {
                context.Set(Value, cachedValue?.ToString());
                context.Set(Found, true);
            }
            else
            {
                context.Set(Value, defaultValue);
                context.Set(Found, false);
            }
        }
        catch (Exception ex)
        {
            context.Set(Value, null);
            context.Set(Found, false);
        }
    }
}
