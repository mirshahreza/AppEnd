using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AppEndServer.Workflows
{
    /// <summary>
    /// Approval Activity - Creates human approval tasks in workflows
    /// Suspends workflow until approval is completed
    /// </summary>
    public class ApprovalActivity : AppEndActivity
    {
        private readonly ILogger<ApprovalActivity>? _logger;
        private string _approverUserId = string.Empty;
        private string[] _approverRoles = Array.Empty<string>();
        private string _approvalTitle = string.Empty;
        private string _approvalDescription = string.Empty;
        private Dictionary<string, string>? _customFields;
        private int _approvalTimeoutDays = 7;
        private bool _requireAllApprovals = false;
        private int? _minimumApprovalsRequired;

        public override string Category => "Approval";
        public override string DisplayName => "Request Approval";
        public override string Description => "Create human approval task and suspend workflow";

        public ApprovalActivity(ILogger<ApprovalActivity>? logger = null)
        {
            _logger = logger;
            Logger = logger;
        }

        /// <summary>
        /// Gets or sets the approver user ID
        /// If not set, uses approver roles instead
        /// </summary>
        public string ApproverUserId
        {
            get => _approverUserId;
            set => _approverUserId = value;
        }

        /// <summary>
        /// Gets or sets the required approver roles
        /// Any user with one of these roles can approve
        /// </summary>
        public string[] ApproverRoles
        {
            get => _approverRoles;
            set => _approverRoles = value ?? Array.Empty<string>();
        }

        /// <summary>
        /// Gets or sets the approval task title
        /// </summary>
        public string ApprovalTitle
        {
            get => _approvalTitle;
            set => _approvalTitle = value;
        }

        /// <summary>
        /// Gets or sets the approval task description
        /// </summary>
        public string ApprovalDescription
        {
            get => _approvalDescription;
            set => _approvalDescription = value;
        }

        /// <summary>
        /// Gets or sets custom fields to display in approval UI
        /// </summary>
        public Dictionary<string, string>? CustomFields
        {
            get => _customFields;
            set => _customFields = value;
        }

        /// <summary>
        /// Gets or sets the approval timeout in days
        /// </summary>
        public int ApprovalTimeoutDays
        {
            get => _approvalTimeoutDays;
            set => _approvalTimeoutDays = Math.Max(1, value);
        }

        /// <summary>
        /// Gets or sets whether all specified approvers must approve
        /// </summary>
        public bool RequireAllApprovals
        {
            get => _requireAllApprovals;
            set => _requireAllApprovals = value;
        }

        /// <summary>
        /// Gets or sets minimum number of approvals required
        /// Only used if RequireAllApprovals is false
        /// </summary>
        public int? MinimumApprovalsRequired
        {
            get => _minimumApprovalsRequired;
            set => _minimumApprovalsRequired = value;
        }

        public override IEnumerable<string> Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(_approverUserId) && (!_approverRoles?.Any() ?? true))
            {
                errors.Add("Either ApproverUserId or ApproverRoles must be specified");
            }

            if (string.IsNullOrWhiteSpace(_approvalTitle))
            {
                errors.Add("ApprovalTitle is required");
            }

            if (_approvalTimeoutDays < 1)
            {
                errors.Add("ApprovalTimeoutDays must be at least 1");
            }

            return errors;
        }

        public override async Task<ActivityExecutionResult> ExecuteAsync(ActivityExecutionContext context)
        {
            var startTime = DateTime.UtcNow;

            try
            {
                _logger?.LogInformation(
                    "ApprovalActivity: Creating approval task '{Title}' for instance {InstanceId}",
                    _approvalTitle, context.WorkflowInstanceId);

                // Validate configuration
                var validationErrors = Validate().ToList();
                if (validationErrors.Any())
                {
                    var errorMessage = string.Join("; ", validationErrors);
                    _logger?.LogError(
                        "ApprovalActivity: Validation failed for approval task '{Title}': {Errors}",
                        _approvalTitle, errorMessage);
                    return ActivityExecutionResult.Failure(errorMessage);
                }

                var approvalTaskId = Guid.NewGuid().ToString();
                var expiresAt = DateTime.UtcNow.AddDays(_approvalTimeoutDays);

                try
                {
                    // Create approval task record
                    _logger?.LogInformation(
                        "ApprovalActivity: Creating approval task record in database");

                    // TODO: Create approval task in database
                    // - Insert into ApprovalTasks table
                    // - Set approver(s) based on UserId or Roles
                    // - Store workflow context (instance ID, correlation ID)
                    // - Set expiration date
                    // - Create workflow bookmark for resumption

                    // Determine approvers
                    var approvers = new List<string>();
                    if (!string.IsNullOrWhiteSpace(_approverUserId))
                    {
                        approvers.Add(_approverUserId);
                        _logger?.LogDebug(
                            "ApprovalActivity: Single approver configured: {ApproverUserId}",
                            _approverUserId);
                    }
                    else if (_approverRoles?.Any() ?? false)
                    {
                        _logger?.LogDebug(
                            "ApprovalActivity: Role-based approval configured for roles: {Roles}",
                            string.Join(", ", _approverRoles));
                    }

                    // Create bookmark for workflow resumption
                    var bookmarkId = Guid.NewGuid().ToString();
                    _logger?.LogDebug(
                        "ApprovalActivity: Creating workflow bookmark {BookmarkId} for resumption",
                        bookmarkId);

                    // TODO: Create bookmark in Elsa
                    // - Store bookmark ID
                    // - Associate with workflow instance
                    // - Set resumption data

                    // Send notifications to approvers
                    if (approvers.Any())
                    {
                        _logger?.LogInformation(
                            "ApprovalActivity: Sending notifications to {ApproverCount} approver(s)",
                            approvers.Count);

                        // TODO: Send email/in-app notifications
                        // foreach (var approver in approvers)
                        // {
                        //     await _notificationService.SendApprovalNotificationAsync(
                        //         approver, _approvalTitle, _approvalDescription);
                        // }
                    }

                    // Store approval task ID in workflow variables for later reference
                    context.SetVariable("CurrentApprovalTaskId", approvalTaskId);
                    context.SetVariable("ApprovalBookmarkId", bookmarkId);
                    context.SetVariable("ApprovalExpiresAt", expiresAt);

                    _logger?.LogInformation(
                        "ApprovalActivity: Approval task '{Title}' created with ID {TaskId} for instance {InstanceId}",
                        _approvalTitle, approvalTaskId, context.WorkflowInstanceId);
                }
                catch (Exception taskCreationEx)
                {
                    _logger?.LogError(taskCreationEx,
                        "ApprovalActivity: Failed to create approval task '{Title}'",
                        _approvalTitle);
                    throw;
                }

                var output = new Dictionary<string, object>
                {
                    { "ApprovalTaskId", approvalTaskId },
                    { "ApprovalTitle", _approvalTitle },
                    { "ApprovalDescription", _approvalDescription },
                    { "Approvers", _approverRoles?.Length > 0 ? string.Join(", ", _approverRoles) : _approverUserId },
                    { "RequireAllApprovals", _requireAllApprovals },
                    { "CreatedAt", DateTime.UtcNow },
                    { "ExpiresAt", expiresAt },
                    { "Status", "Pending" },
                    { "CustomFields", _customFields ?? new Dictionary<string, string>() }
                };

                // Return suspended state - workflow will be paused here
                var result = ActivityExecutionResult.SuccessResult(output);
                result.CustomData["WorkflowSuspended"] = true;
                result.CustomData["BookmarkId"] = Guid.NewGuid().ToString();
                result.CustomData["ResumptionReason"] = $"Awaiting approval for: {_approvalTitle}";

                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex,
                    "ApprovalActivity: Failed to create approval task '{Title}' for instance {InstanceId}",
                    _approvalTitle, context.WorkflowInstanceId);

                return ActivityExecutionResult.Failure(
                    $"Approval task creation failed: {ex.Message}",
                    ex);
            }
            finally
            {
                var duration = DateTime.UtcNow - startTime;
                _logger?.LogDebug(
                    "ApprovalActivity: Approval task creation for '{Title}' completed in {Duration}ms",
                    _approvalTitle, duration.TotalMilliseconds);
            }
        }

        public override void Initialize()
        {
            _logger?.LogDebug(
                "ApprovalActivity initialized for approval '{Title}' with {RoleCount} roles",
                _approvalTitle, _approverRoles.Length);
        }

        public override void Dispose()
        {
            _customFields?.Clear();
            _logger?.LogDebug("ApprovalActivity disposed");
        }
    }

    /// <summary>
    /// Configuration for approval activities
    /// </summary>
    public class ApprovalActivityOptions
    {
        /// <summary>
        /// Default approval timeout in days
        /// </summary>
        public int DefaultApprovalTimeoutDays { get; set; } = 7;

        /// <summary>
        /// Maximum approval timeout allowed in days
        /// </summary>
        public int MaxApprovalTimeoutDays { get; set; } = 90;

        /// <summary>
        /// Whether to send email notifications to approvers
        /// </summary>
        public bool SendEmailNotifications { get; set; } = true;

        /// <summary>
        /// Whether to send in-app notifications to approvers
        /// </summary>
        public bool SendAppNotifications { get; set; } = true;

        /// <summary>
        /// Whether to automatically reject after timeout
        /// </summary>
        public bool AutoRejectOnTimeout { get; set; } = false;

        /// <summary>
        /// Whether to escalate to manager on timeout
        /// </summary>
        public bool EscalateOnTimeout { get; set; } = true;

        /// <summary>
        /// Escalation recipient role when approval times out
        /// </summary>
        public string EscalationRole { get; set; } = "admin";

        /// <summary>
        /// Audit all approval decisions
        /// </summary>
        public bool AuditDecisions { get; set; } = true;

        /// <summary>
        /// Require comment on rejection
        /// </summary>
        public bool RequireRejectionReason { get; set; } = true;

        /// <summary>
        /// Directory path for approval templates
        /// </summary>
        public string TemplateDirectory { get; set; } = "Workflows/Templates/Approvals";
    }

    /// <summary>
    /// Approval decision models
    /// </summary>
    public class ApprovalDecision
    {
        public enum DecisionType
        {
            Pending = 0,
            Approved = 1,
            Rejected = 2,
            Escalated = 3
        }

        public string ApprovalTaskId { get; set; } = string.Empty;
        public string ApproversDecision { get; set; } = string.Empty;
        public DecisionType Decision { get; set; }
        public string? RejectionReason { get; set; }
        public string ApproverUserId { get; set; } = string.Empty;
        public DateTime DecisionTime { get; set; }
        public Dictionary<string, object>? ApprovalData { get; set; }
    }
}
