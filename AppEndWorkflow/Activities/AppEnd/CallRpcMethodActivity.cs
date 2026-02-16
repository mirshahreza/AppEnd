using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace AppEndWorkflow.Activities.AppEnd;

/// <summary>
/// Calls an existing static RPC method dynamically using reflection.
/// Allows workflows to invoke any existing AppEnd business logic without code duplication.
/// </summary>
[Activity(
    Category = "AppEnd",
    Description = "Calls an existing static RPC method using reflection",
    DisplayName = "Call RPC Method"
)]
public class CallRpcMethodActivity : CodeActivity
{
    /// <summary>
    /// Name of the static RPC method (e.g., "GetOrderById", "ValidateOrder")
    /// </summary>
    [Input(Description = "Name of the static RPC method (e.g., 'GetOrderById')")]
    public Input<string> MethodName { get; set; } = default!;

    /// <summary>
    /// JSON string of input parameters
    /// </summary>
    [Input(Description = "JSON string of input parameters")]
    public Input<string?> InputParams { get; set; }

    /// <summary>
    /// JSON result from the method
    /// </summary>
    [Output(Description = "JSON result from the method")]
    public Output<string> Result { get; set; } = default!;

    /// <summary>
    /// Whether the call succeeded
    /// </summary>
    [Output(Description = "Whether the call succeeded")]
    public Output<bool> Success { get; set; } = default!;

    /// <summary>
    /// Error message if failed
    /// </summary>
    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var methodName = context.Get(MethodName);
            var inputParamsJson = context.Get(InputParams);

            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentException("'MethodName' is required");

            // Invoke the RPC method using reflection
            var result = InvokeRpcMethod(methodName, inputParamsJson);

            context.Set(Result, result);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(Result, "{}");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private string InvokeRpcMethod(string methodName, string? inputParamsJson)
    {
        // TODO: Implement RPC method invocation
        // This should use AppEnd's existing RPC mechanism to invoke static methods
        // 
        // Possible implementation approaches:
        // 1. Use AppEnd's RPC registry if available (e.g., RpcRegistry.Invoke)
        // 2. Reflection-based method lookup in known assemblies
        // 3. Integration with Zzz.AppEndProxy or similar RPC entry point
        // 
        // For now, this is a placeholder that demonstrates the structure

        // Parse input parameters if provided
        Dictionary<string, object>? parameters = null;
        if (!string.IsNullOrWhiteSpace(inputParamsJson))
        {
            try
            {
                using var doc = JsonDocument.Parse(inputParamsJson);
                parameters = new Dictionary<string, object>();
                foreach (var property in doc.RootElement.EnumerateObject())
                {
                    parameters[property.Name] = property.Value;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse input parameters: {ex.Message}");
            }
        }

        // TODO: Call the RPC method using AppEnd's RPC infrastructure
        // Result should be serialized to JSON
        
        throw new NotImplementedException("RPC method invocation needs to be implemented with AppEnd's RPC infrastructure");
    }
}
