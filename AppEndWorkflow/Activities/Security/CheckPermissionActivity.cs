using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.Security;

/// <summary>
/// Checks user permissions in AppEnd's permission/role system.
/// </summary>
[Activity(
    Category = "Security",
    Description = "Checks user permissions",
    DisplayName = "Check Permission"
)]
public class CheckPermissionActivity : CodeActivity
{
    /// <summary>
    /// User to check
    /// </summary>
    [Input(Description = "User ID or username")]
    public Input<string> UserId { get; set; } = default!;

    /// <summary>
    /// Permission name or resource path
    /// </summary>
    [Input(Description = "Permission name or resource path")]
    public Input<string> Permission { get; set; } = default!;

    /// <summary>
    /// Action type: "Read", "Write", "Delete", "Execute"
    /// </summary>
    [Input(Description = "Action type: Read, Write, Delete, Execute")]
    public Input<string> Action { get; set; } = new("Read");

    /// <summary>
    /// Whether user has the permission
    /// </summary>
    [Output(Description = "Whether user has the permission")]
    public Output<bool> HasPermission { get; set; } = default!;

    /// <summary>
    /// JSON array of user's roles
    /// </summary>
    [Output(Description = "JSON array of user's roles")]
    public Output<string> UserRoles { get; set; } = default!;

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var userId = context.Get(UserId);
            var permission = context.Get(Permission);
            var action = context.Get(Action);

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("'UserId' is required");

            if (string.IsNullOrWhiteSpace(permission))
                throw new ArgumentException("'Permission' is required");

            if (string.IsNullOrWhiteSpace(action))
                action = "Read";

            // TODO: Implement permission checking with AppEnd's permission system
            // This should:
            // 1. Query user's roles
            // 2. Query role permissions
            // 3. Check if user has the required permission
            //
            // For now, this is a placeholder
            var (hasPermission, userRoles) = CheckPermissionInternal(userId, permission, action);

            context.Set(HasPermission, hasPermission);
            context.Set(UserRoles, System.Text.Json.JsonSerializer.Serialize(userRoles));
        }
        catch (Exception ex)
        {
            context.Set(HasPermission, false);
            context.Set(UserRoles, "[]");
        }
    }

    private (bool hasPermission, List<string> userRoles) CheckPermissionInternal(
        string userId, string permission, string action)
    {
        // TODO: Implement with AppEnd's permission/role system
        // Placeholder: return empty roles and no permission
        return (false, new List<string>());
    }
}
