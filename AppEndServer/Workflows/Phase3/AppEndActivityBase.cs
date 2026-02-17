using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AppEndServer.Workflows
{
    /// <summary>
    /// Base class for all AppEnd workflow activities
    /// Provides common functionality and context for activity execution
    /// </summary>
    public abstract class AppEndActivity
    {
        /// <summary>
        /// Gets the activity identifier (typically the activity type name)
        /// </summary>
        public virtual string ActivityId => GetType().Name;

        /// <summary>
        /// Gets the activity display name for UI and logging
        /// </summary>
        public virtual string DisplayName => ActivityId;

        /// <summary>
        /// Gets the activity description
        /// </summary>
        public virtual string Description => $"{ActivityId} activity for workflow execution";

        /// <summary>
        /// Gets the activity category (e.g., "Database", "Notification", "Code")
        /// </summary>
        public abstract string Category { get; }

        /// <summary>
        /// Gets the activity version
        /// </summary>
        public virtual string Version => "1.0.0";

        /// <summary>
        /// Gets whether this activity supports async execution
        /// </summary>
        public virtual bool SupportsAsync => true;

        /// <summary>
        /// Gets whether this activity can have outbound connections
        /// </summary>
        public virtual bool AllowOutboundConnections => true;

        /// <summary>
        /// Gets the logger for this activity
        /// </summary>
        protected ILogger? Logger { get; set; }

        /// <summary>
        /// Executes the activity synchronously
        /// Override this method to implement activity logic
        /// </summary>
        /// <param name="context">The activity execution context</param>
        /// <returns>Activity execution result</returns>
        public virtual ActivityExecutionResult Execute(ActivityExecutionContext context)
        {
            throw new NotImplementedException(
                $"Activity {ActivityId} does not implement synchronous execution. " +
                "Override Execute() or ExecuteAsync().");
        }

        /// <summary>
        /// Executes the activity asynchronously
        /// Override this method to implement activity logic
        /// </summary>
        /// <param name="context">The activity execution context</param>
        /// <returns>Task returning activity execution result</returns>
        public virtual async Task<ActivityExecutionResult> ExecuteAsync(ActivityExecutionContext context)
        {
            return await Task.FromResult(Execute(context));
        }

        /// <summary>
        /// Validates the activity configuration before execution
        /// Override to implement custom validation
        /// </summary>
        /// <returns>List of validation errors (empty if valid)</returns>
        public virtual IEnumerable<string> Validate()
        {
            return new List<string>();
        }

        /// <summary>
        /// Called when the activity is being initialized
        /// Override to implement custom initialization
        /// </summary>
        public virtual void Initialize()
        {
            // Override in derived class if needed
        }

        /// <summary>
        /// Called when the activity is being disposed
        /// Override to clean up resources
        /// </summary>
        public virtual void Dispose()
        {
            // Override in derived class if needed
        }
    }

    /// <summary>
    /// Context for activity execution containing workflow and instance information
    /// </summary>
    public class ActivityExecutionContext
    {
        /// <summary>
        /// Gets the workflow instance ID
        /// </summary>
        public string WorkflowInstanceId { get; set; } = string.Empty;

        /// <summary>
        /// Gets the workflow definition ID
        /// </summary>
        public string WorkflowDefinitionId { get; set; } = string.Empty;

        /// <summary>
        /// Gets the activity execution ID
        /// </summary>
        public string ActivityExecutionId { get; set; } = string.Empty;

        /// <summary>
        /// Gets the workflow variables/state
        /// </summary>
        public Dictionary<string, object> Variables { get; set; } = new();

        /// <summary>
        /// Gets the activity input parameters
        /// </summary>
        public Dictionary<string, object> Input { get; set; } = new();

        /// <summary>
        /// Gets or sets the activity output data
        /// </summary>
        public Dictionary<string, object> Output { get; set; } = new();

        /// <summary>
        /// Gets the tenant ID (multi-tenancy support)
        /// </summary>
        public string? TenantId { get; set; }

        /// <summary>
        /// Gets the user ID that triggered the workflow
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Gets the correlation ID for grouping related instances
        /// </summary>
        public string CorrelationId { get; set; } = string.Empty;

        /// <summary>
        /// Gets the current execution timestamp
        /// </summary>
        public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets a value from the workflow variables
        /// </summary>
        public T? GetVariable<T>(string name, T? defaultValue = default)
        {
            if (Variables.TryGetValue(name, out var value))
            {
                if (value is T typedValue)
                    return typedValue;

                // Try to convert if types don't match
                try
                {
                    return (T?)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Sets a variable in the workflow state
        /// </summary>
        public void SetVariable(string name, object? value)
        {
            if (value == null)
            {
                Variables.Remove(name);
            }
            else
            {
                Variables[name] = value;
            }
        }
    }

    /// <summary>
    /// Result of activity execution
    /// </summary>
    public class ActivityExecutionResult
    {
        /// <summary>
        /// Gets or sets whether the activity executed successfully
        /// </summary>
        public bool IsSuccess { get; set; } = true;

        /// <summary>
        /// Gets or sets the activity output data
        /// </summary>
        public Dictionary<string, object> Output { get; set; } = new();

        /// <summary>
        /// Gets or sets the error message (if execution failed)
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the exception (if execution failed)
        /// </summary>
        public Exception? Exception { get; set; }

        /// <summary>
        /// Gets or sets the next activity to execute
        /// If null, workflow will look for normal outbound connection
        /// </summary>
        public string? NextActivityId { get; set; }

        /// <summary>
        /// Gets or sets custom data associated with the execution
        /// </summary>
        public Dictionary<string, object> CustomData { get; set; } = new();

        /// <summary>
        /// Gets the execution duration
        /// </summary>
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// Creates a successful result with output data
        /// </summary>
        public static ActivityExecutionResult SuccessResult(Dictionary<string, object>? output = null)
        {
            return new ActivityExecutionResult
            {
                IsSuccess = true,
                Output = output ?? new Dictionary<string, object>()
            };
        }

        /// <summary>
        /// Creates a failed result with error information
        /// </summary>
        public static ActivityExecutionResult Failure(string errorMessage, Exception? exception = null)
        {
            return new ActivityExecutionResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                Exception = exception
            };
        }

        /// <summary>
        /// Creates a result that branches to another activity
        /// </summary>
        public static ActivityExecutionResult Branch(string nextActivityId, Dictionary<string, object>? output = null)
        {
            return new ActivityExecutionResult
            {
                IsSuccess = true,
                NextActivityId = nextActivityId,
                Output = output ?? new Dictionary<string, object>()
            };
        }
    }

    /// <summary>
    /// Registry for managing custom activities
    /// Allows registration, discovery, and instantiation of activities
    /// </summary>
    public class ActivityRegistry
    {
        private readonly Dictionary<string, Type> _registeredActivities = new();
        private readonly Dictionary<string, object> _singletonInstances = new();
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ActivityRegistry> _logger;

        public ActivityRegistry(IServiceProvider serviceProvider, ILogger<ActivityRegistry> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Registers an activity type
        /// </summary>
        public void RegisterActivity<T>(string? activityId = null) where T : AppEndActivity
        {
            var id = activityId ?? typeof(T).Name;
            _registeredActivities[id] = typeof(T);
            _logger.LogInformation("Registered activity: {ActivityId} -> {ActivityType}", id, typeof(T).FullName);
        }

        /// <summary>
        /// Registers a singleton activity instance
        /// </summary>
        public void RegisterActivitySingleton<T>(T instance, string? activityId = null) where T : AppEndActivity
        {
            var id = activityId ?? typeof(T).Name;
            _registeredActivities[id] = typeof(T);
            _singletonInstances[id] = instance;
            _logger.LogInformation("Registered singleton activity: {ActivityId}", id);
        }

        /// <summary>
        /// Gets the type for a registered activity
        /// </summary>
        public Type? GetActivityType(string activityId)
        {
            return _registeredActivities.TryGetValue(activityId, out var type) ? type : null;
        }

        /// <summary>
        /// Gets a list of all registered activity IDs
        /// </summary>
        public IEnumerable<string> GetRegisteredActivityIds()
        {
            return _registeredActivities.Keys;
        }

        /// <summary>
        /// Creates an instance of a registered activity
        /// </summary>
        public AppEndActivity? CreateActivity(string activityId)
        {
            // Check if singleton instance exists
            if (_singletonInstances.TryGetValue(activityId, out var singleton))
            {
                return singleton as AppEndActivity;
            }

            // Get activity type
            if (!_registeredActivities.TryGetValue(activityId, out var type))
            {
                _logger.LogWarning("Activity not found: {ActivityId}", activityId);
                return null;
            }

            try
            {
                // Try to resolve from DI container
                var instance = _serviceProvider.GetService(type) as AppEndActivity;
                if (instance != null)
                {
                    instance.Initialize();
                    return instance;
                }

                // Fall back to direct instantiation
                instance = Activator.CreateInstance(type) as AppEndActivity;
                if (instance != null)
                {
                    instance.Initialize();
                }

                return instance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create activity instance: {ActivityId}", activityId);
                return null;
            }
        }

        /// <summary>
        /// Gets metadata for all registered activities
        /// </summary>
        public IEnumerable<ActivityMetadata> GetActivityMetadata()
        {
            var metadata = new List<ActivityMetadata>();

            foreach (var (activityId, type) in _registeredActivities)
            {
                try
                {
                    var instance = CreateActivity(activityId);
                    if (instance != null)
                    {
                        metadata.Add(new ActivityMetadata
                        {
                            ActivityId = activityId,
                            DisplayName = instance.DisplayName,
                            Description = instance.Description,
                            Category = instance.Category,
                            Version = instance.Version,
                            SupportsAsync = instance.SupportsAsync,
                            AllowOutboundConnections = instance.AllowOutboundConnections,
                            Type = type.FullName ?? type.Name
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to get metadata for activity: {ActivityId}", activityId);
                }
            }

            return metadata;
        }
    }

    /// <summary>
    /// Metadata about a registered activity
    /// </summary>
    public class ActivityMetadata
    {
        public string ActivityId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public bool SupportsAsync { get; set; } = true;
        public bool AllowOutboundConnections { get; set; } = true;
        public string Type { get; set; } = string.Empty;
    }
}
