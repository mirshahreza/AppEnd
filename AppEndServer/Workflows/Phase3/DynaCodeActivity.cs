using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppEndDynaCode;
using Microsoft.Extensions.Logging;

namespace AppEndServer.Workflows
{
    /// <summary>
    /// DynaCode Activity - Executes dynamic C# code within workflows
    /// Allows workflows to invoke compiled DynaCode methods
    /// </summary>
    public class DynaCodeActivity : AppEndActivity
    {
        private readonly ILogger<DynaCodeActivity>? _logger;
        private string _methodFullName = string.Empty;
        private Dictionary<string, object>? _methodParameters;
        private int? _executionTimeoutMs;

        public override string Category => "Code";
        public override string DisplayName => "Execute Code";
        public override string Description => "Execute dynamic C# code from DynaCode assembly";

        public DynaCodeActivity(ILogger<DynaCodeActivity>? logger = null)
        {
            _logger = logger;
            Logger = logger;
        }

        /// <summary>
        /// Gets or sets the full method name to invoke
        /// Format: Namespace.ClassName.MethodName
        /// </summary>
        public string MethodFullName
        {
            get => _methodFullName;
            set => _methodFullName = value;
        }

        /// <summary>
        /// Gets or sets the method parameters
        /// </summary>
        public Dictionary<string, object>? MethodParameters
        {
            get => _methodParameters;
            set => _methodParameters = value;
        }

        /// <summary>
        /// Gets or sets the execution timeout in milliseconds
        /// </summary>
        public int? ExecutionTimeoutMs
        {
            get => _executionTimeoutMs;
            set => _executionTimeoutMs = value;
        }

        public override IEnumerable<string> Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(_methodFullName))
            {
                errors.Add("MethodFullName is required");
            }

            if (!_methodFullName.Contains("."))
            {
                errors.Add("MethodFullName must be in format: Namespace.ClassName.MethodName");
            }

            return errors;
        }

        public override async Task<ActivityExecutionResult> ExecuteAsync(ActivityExecutionContext context)
        {
            var startTime = DateTime.UtcNow;
            var tokenSource = new System.Threading.CancellationTokenSource();

            try
            {
                _logger?.LogInformation(
                    "DynaCodeActivity: Invoking method '{MethodName}' for instance {InstanceId}",
                    _methodFullName, context.WorkflowInstanceId);

                // Validate configuration
                var validationErrors = Validate().ToList();
                if (validationErrors.Any())
                {
                    var errorMessage = string.Join("; ", validationErrors);
                    _logger?.LogError(
                        "DynaCodeActivity: Validation failed for method '{MethodName}': {Errors}",
                        _methodFullName, errorMessage);
                    return ActivityExecutionResult.Failure(errorMessage);
                }

                // Set execution timeout if specified
                if (_executionTimeoutMs.HasValue)
                {
                    tokenSource.CancelAfter(_executionTimeoutMs.Value);
                    _logger?.LogDebug(
                        "DynaCodeActivity: Set execution timeout to {Timeout}ms",
                        _executionTimeoutMs.Value);
                }

                object? returnValue = null;

                try
                {
                    // Parse MethodFullName (Namespace.Class.Method)
                    var parts = _methodFullName.Split('.');
                    if (parts.Length < 3)
                    {
                        throw new InvalidOperationException(
                            $"Invalid method name format: {_methodFullName}. Expected: Namespace.Class.Method");
                    }

                    var methodName = parts[^1];
                    var className = parts[^2];
                    var namespaceName = string.Join(".", parts.Take(parts.Length - 2));

                    _logger?.LogDebug(
                        "DynaCodeActivity: Parsed method (Namespace: {Namespace}, Class: {Class}, Method: {Method})",
                        namespaceName, className, methodName);

                    // Get type from DynaCode assembly
                    var assembly = AppEndDynaCode.DynaCode.DynaAsm;
                    var type = assembly.GetType($"{namespaceName}.{className}");

                    if (type == null)
                    {
                        throw new InvalidOperationException(
                            $"Type '{namespaceName}.{className}' not found in DynaCode assembly");
                    }

                    // Find method with matching name
                    var method = type.GetMethod(methodName, 
                        System.Reflection.BindingFlags.Public | 
                        System.Reflection.BindingFlags.Static |
                        System.Reflection.BindingFlags.Instance);

                    if (method == null)
                    {
                        throw new InvalidOperationException(
                            $"Method '{methodName}' not found in type '{namespaceName}.{className}'");
                    }

                    // Prepare method parameters
                    var methodParams = method.GetParameters();
                    var paramValues = new object?[methodParams.Length];

                    if (_methodParameters != null)
                    {
                        for (int i = 0; i < methodParams.Length; i++)
                        {
                            var paramName = methodParams[i].Name;
                            if (_methodParameters.TryGetValue(paramName, out var paramValue))
                            {
                                paramValues[i] = paramValue;
                                _logger?.LogDebug(
                                    "DynaCodeActivity: Parameter {ParamName} = {ParamValue}",
                                    paramName, paramValue ?? "null");
                            }
                        }
                    }

                    // Invoke method
                    _logger?.LogInformation(
                        "DynaCodeActivity: Invoking method {MethodFullName} with {ParamCount} parameters",
                        _methodFullName, paramValues.Length);

                    if (method.IsStatic)
                    {
                        returnValue = method.Invoke(null, paramValues);
                    }
                    else
                    {
                        var instance = Activator.CreateInstance(type);
                        returnValue = method.Invoke(instance, paramValues);
                    }

                    // Handle async methods
                    if (method.ReturnType.IsGenericType && 
                        method.ReturnType.GetGenericTypeDefinition() == typeof(System.Threading.Tasks.Task<>))
                    {
                        var taskType = method.ReturnType;
                        var resultProperty = taskType.GetProperty("Result");
                        returnValue = resultProperty?.GetValue(returnValue);
                    }

                    _logger?.LogInformation(
                        "DynaCodeActivity: Method '{MethodName}' invoked successfully for instance {InstanceId}",
                        _methodFullName, context.WorkflowInstanceId);
                }
                catch (System.OperationCanceledException ex)
                {
                    _logger?.LogError(ex,
                        "DynaCodeActivity: Method execution timeout for '{MethodName}'",
                        _methodFullName);
                    throw new InvalidOperationException(
                        $"DynaCode method execution timeout after {_executionTimeoutMs}ms", ex);
                }

                var output = new Dictionary<string, object>
                {
                    { "MethodFullName", _methodFullName },
                    { "ReturnValue", returnValue ?? "null" },
                    { "ReturnType", returnValue?.GetType().Name ?? "void" },
                    { "ParameterCount", _methodParameters?.Count ?? 0 },
                    { "ExecutedAt", DateTime.UtcNow }
                };

                return ActivityExecutionResult.SuccessResult(output);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex,
                    "DynaCodeActivity: Failed to invoke method '{MethodName}' for instance {InstanceId}",
                    _methodFullName, context.WorkflowInstanceId);

                return ActivityExecutionResult.Failure(
                    $"DynaCode execution failed: {ex.Message}",
                    ex);
            }
            finally
            {
                var duration = DateTime.UtcNow - startTime;
                _logger?.LogDebug(
                    "DynaCodeActivity: Method '{MethodName}' completed in {Duration}ms",
                    _methodFullName, duration.TotalMilliseconds);
                
                tokenSource.Dispose();
            }
        }

        public override void Initialize()
        {
            _logger?.LogDebug("DynaCodeActivity initialized for method '{MethodName}'", _methodFullName);
        }

        public override void Dispose()
        {
            _methodParameters?.Clear();
            _logger?.LogDebug("DynaCodeActivity disposed for method '{MethodName}'", _methodFullName);
        }
    }

    /// <summary>
    /// Configuration for DynaCode activities
    /// </summary>
    public class DynaCodeActivityOptions
    {
        /// <summary>
        /// Default execution timeout in milliseconds
        /// </summary>
        public int DefaultExecutionTimeoutMs { get; set; } = 30000;

        /// <summary>
        /// Maximum execution timeout allowed
        /// </summary>
        public int MaxExecutionTimeoutMs { get; set; } = 300000;

        /// <summary>
        /// Whether to cache method reflections
        /// </summary>
        public bool EnableMethodCaching { get; set; } = true;

        /// <summary>
        /// Whether to allow void method execution
        /// </summary>
        public bool AllowVoidMethods { get; set; } = true;

        /// <summary>
        /// Namespace prefixes allowed for execution
        /// Empty list means all namespaces allowed
        /// </summary>
        public List<string> AllowedNamespacePrefixes { get; set; } = new();

        /// <summary>
        /// Whether to pass workflow context to methods
        /// </summary>
        public bool PassWorkflowContext { get; set; } = true;

        /// <summary>
        /// Whether to pass activity execution context to methods
        /// </summary>
        public bool PassActivityContext { get; set; } = true;
    }
}
