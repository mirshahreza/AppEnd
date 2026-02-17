using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AppEndServer.Workflows
{
    /// <summary>
    /// Manages workflow activity execution and lifecycle
    /// Orchestrates activity registry, execution, and result handling
    /// </summary>
    public class ActivityManager
    {
        private readonly ActivityRegistry _registry;
        private readonly ILogger<ActivityManager> _logger;
        private readonly Dictionary<string, AppEndActivity> _executingActivities = new();

        public ActivityManager(ActivityRegistry registry, ILogger<ActivityManager> logger)
        {
            _registry = registry;
            _logger = logger;
        }

        /// <summary>
        /// Executes an activity by ID
        /// </summary>
        public async Task<ActivityExecutionResult> ExecuteActivityAsync(
            string activityId,
            ActivityExecutionContext context)
        {
            var startTime = DateTime.UtcNow;
            AppEndActivity? activity = null;

            try
            {
                _logger.LogInformation(
                    "ActivityManager: Executing activity {ActivityId} for instance {InstanceId}",
                    activityId, context.WorkflowInstanceId);

                // Get activity instance
                activity = _registry.CreateActivity(activityId);
                if (activity == null)
                {
                    var error = $"Activity {activityId} not found or could not be instantiated";
                    _logger.LogError("ActivityManager: {Error}", error);
                    return ActivityExecutionResult.Failure(error);
                }

                // Track executing activity
                _executingActivities[context.ActivityExecutionId] = activity;

                // Validate activity
                var validationErrors = activity.Validate().ToList();
                if (validationErrors.Any())
                {
                    var error = $"Activity validation failed: {string.Join("; ", validationErrors)}";
                    _logger.LogWarning("ActivityManager: {Error}", error);
                    return ActivityExecutionResult.Failure(error);
                }

                // Execute activity
                ActivityExecutionResult result;
                if (activity.SupportsAsync)
                {
                    result = await activity.ExecuteAsync(context);
                }
                else
                {
                    result = activity.Execute(context);
                }

                // Add execution duration
                result.Duration = DateTime.UtcNow - startTime;

                _logger.LogInformation(
                    "ActivityManager: Activity {ActivityId} completed with success={Success} in {Duration}ms",
                    activityId, result.IsSuccess, result.Duration?.TotalMilliseconds ?? 0);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "ActivityManager: Unexpected error executing activity {ActivityId} for instance {InstanceId}",
                    activityId, context.WorkflowInstanceId);

                return ActivityExecutionResult.Failure(
                    $"Unexpected error: {ex.Message}",
                    ex);
            }
            finally
            {
                // Clean up
                if (activity != null)
                {
                    try
                    {
                        activity.Dispose();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error disposing activity {ActivityId}", activityId);
                    }
                }

                _executingActivities.Remove(context.ActivityExecutionId);
            }
        }

        /// <summary>
        /// Gets activity metadata for discovery and UI
        /// </summary>
        public IEnumerable<ActivityMetadata> GetActivityMetadata()
        {
            return _registry.GetActivityMetadata();
        }

        /// <summary>
        /// Gets metadata for a specific activity
        /// </summary>
        public ActivityMetadata? GetActivityMetadata(string activityId)
        {
            return _registry.GetActivityMetadata()
                .FirstOrDefault(m => m.ActivityId == activityId);
        }

        /// <summary>
        /// Lists all registered activity IDs
        /// </summary>
        public IEnumerable<string> GetRegisteredActivityIds()
        {
            return _registry.GetRegisteredActivityIds();
        }

        /// <summary>
        /// Groups activities by category
        /// </summary>
        public IEnumerable<IGrouping<string, ActivityMetadata>> GetActivitiesByCategory()
        {
            return _registry.GetActivityMetadata()
                .GroupBy(m => m.Category);
        }

        /// <summary>
        /// Gets the status of currently executing activities
        /// </summary>
        public IEnumerable<(string ExecutionId, string ActivityId)> GetExecutingActivities()
        {
            return _executingActivities
                .Select(kvp => (kvp.Key, kvp.Value.ActivityId));
        }

        /// <summary>
        /// Cancels an executing activity
        /// </summary>
        public void CancelActivity(string executionId)
        {
            if (_executingActivities.TryGetValue(executionId, out var activity))
            {
                _logger.LogInformation(
                    "ActivityManager: Cancelling executing activity {ActivityId}",
                    activity.ActivityId);

                try
                {
                    activity.Dispose();
                    _executingActivities.Remove(executionId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "ActivityManager: Error cancelling activity {ActivityId}",
                        activity.ActivityId);
                }
            }
        }
    }

    /// <summary>
    /// Extension for registering Phase 3 custom activities in DI
    /// </summary>
    public static class ActivityServiceCollectionExtensions
    {
        /// <summary>
        /// Adds workflow activities to the service collection
        /// </summary>
        public static IServiceCollection AddWorkflowActivities(
            this IServiceCollection services,
            Action<ActivityRegistrationBuilder>? configure = null)
        {
            // Register activity registry
            services.AddSingleton<ActivityRegistry>();
            services.AddScoped<ActivityManager>();

            // Register activity configurations
            services.Configure<DatabaseActivityOptions>(options =>
            {
                options.DefaultConnectionName = "DefaultRepo";
                options.DefaultCommandTimeout = 30;
            });

            services.Configure<DynaCodeActivityOptions>(options =>
            {
                options.DefaultExecutionTimeoutMs = 30000;
                options.PassWorkflowContext = true;
                options.PassActivityContext = true;
            });

            services.Configure<NotificationActivityOptions>(options =>
            {
                options.EnableEmailNotifications = true;
                options.EnableAppNotifications = true;
                options.DeliveryTimeoutMs = 30000;
                options.RetryAttempts = 3;
            });

            services.Configure<ApprovalActivityOptions>(options =>
            {
                options.DefaultApprovalTimeoutDays = 7;
                options.SendEmailNotifications = true;
                options.SendAppNotifications = true;
                options.AuditDecisions = true;
            });

            // Register default activities
            services.AddScoped<DatabaseActivity>();
            services.AddScoped<DynaCodeActivity>();
            services.AddScoped<NotificationActivity>();
            services.AddScoped<ApprovalActivity>();

            // Register activities with registry
            var serviceProvider = services.BuildServiceProvider();
            var registry = serviceProvider.GetRequiredService<ActivityRegistry>();

            registry.RegisterActivity<DatabaseActivity>();
            registry.RegisterActivity<DynaCodeActivity>();
            registry.RegisterActivity<NotificationActivity>();
            registry.RegisterActivity<ApprovalActivity>();

            // Allow custom registration
            if (configure != null)
            {
                var builder = new ActivityRegistrationBuilder(registry);
                configure(builder);
            }

            return services;
        }
    }

    /// <summary>
    /// Builder for custom activity registration
    /// </summary>
    public class ActivityRegistrationBuilder
    {
        private readonly ActivityRegistry _registry;

        public ActivityRegistrationBuilder(ActivityRegistry registry)
        {
            _registry = registry;
        }

        /// <summary>
        /// Registers a custom activity
        /// </summary>
        public ActivityRegistrationBuilder RegisterActivity<T>(string? activityId = null)
            where T : AppEndActivity
        {
            _registry.RegisterActivity<T>(activityId);
            return this;
        }

        /// <summary>
        /// Registers a singleton activity instance
        /// </summary>
        public ActivityRegistrationBuilder RegisterActivitySingleton<T>(
            T instance,
            string? activityId = null)
            where T : AppEndActivity
        {
            _registry.RegisterActivitySingleton(instance, activityId);
            return this;
        }
    }
}
