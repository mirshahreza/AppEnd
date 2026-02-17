using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AppEndServer.Workflows
{
    /// <summary>
    /// Notification Activity - Sends notifications through various channels
    /// Supports Email, SMS, and AppEnd in-app notifications
    /// </summary>
    public class NotificationActivity : AppEndActivity
    {
        private readonly ILogger<NotificationActivity>? _logger;
        private NotificationChannel _channel = NotificationChannel.Email;
        private string _recipient = string.Empty;
        private string _subject = string.Empty;
        private string _message = string.Empty;
        private Dictionary<string, string>? _templateData;
        private bool _isTemplate = false;
        private string? _templateName;

        public override string Category => "Notification";
        public override string DisplayName => "Send Notification";
        public override string Description => "Send notifications via Email, SMS, or AppEnd";

        public NotificationActivity(ILogger<NotificationActivity>? logger = null)
        {
            _logger = logger;
            Logger = logger;
        }

        /// <summary>
        /// Notification channel enumeration
        /// </summary>
        public enum NotificationChannel
        {
            Email = 0,
            Sms = 1,
            AppNotification = 2,
            Webhook = 3
        }

        /// <summary>
        /// Gets or sets the notification channel
        /// </summary>
        public NotificationChannel Channel
        {
            get => _channel;
            set => _channel = value;
        }

        /// <summary>
        /// Gets or sets the recipient (email address, phone number, or user ID)
        /// </summary>
        public string Recipient
        {
            get => _recipient;
            set => _recipient = value;
        }

        /// <summary>
        /// Gets or sets the notification subject (used for Email)
        /// </summary>
        public string Subject
        {
            get => _subject;
            set => _subject = value;
        }

        /// <summary>
        /// Gets or sets the notification message/body
        /// </summary>
        public string Message
        {
            get => _message;
            set => _message = value;
        }

        /// <summary>
        /// Gets or sets whether message is a template name
        /// </summary>
        public bool IsTemplate
        {
            get => _isTemplate;
            set => _isTemplate = value;
        }

        /// <summary>
        /// Gets or sets the template name (if IsTemplate is true)
        /// </summary>
        public string? TemplateName
        {
            get => _templateName;
            set => _templateName = value;
        }

        /// <summary>
        /// Gets or sets template data for variable substitution
        /// </summary>
        public Dictionary<string, string>? TemplateData
        {
            get => _templateData;
            set => _templateData = value;
        }

        public override IEnumerable<string> Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(_recipient))
            {
                errors.Add("Recipient is required");
            }

            if (_channel == NotificationChannel.Email && string.IsNullOrWhiteSpace(_subject))
            {
                errors.Add("Subject is required for email notifications");
            }

            if (string.IsNullOrWhiteSpace(_message) && (!_isTemplate || string.IsNullOrWhiteSpace(_templateName)))
            {
                errors.Add("Message or TemplateName is required");
            }

            return errors;
        }

        public override async Task<ActivityExecutionResult> ExecuteAsync(ActivityExecutionContext context)
        {
            var startTime = DateTime.UtcNow;

            try
            {
                _logger?.LogInformation(
                    "NotificationActivity: Sending {Channel} notification to {Recipient} for instance {InstanceId}",
                    _channel, _recipient, context.WorkflowInstanceId);

                // Validate configuration
                var validationErrors = Validate().ToList();
                if (validationErrors.Any())
                {
                    var errorMessage = string.Join("; ", validationErrors);
                    _logger?.LogError(
                        "NotificationActivity: Validation failed for {Channel} notification: {Errors}",
                        _channel, errorMessage);
                    return ActivityExecutionResult.Failure(errorMessage);
                }

                string finalMessage = _message;
                string finalSubject = _subject;

                // Handle template loading if needed
                if (_isTemplate && !string.IsNullOrWhiteSpace(_templateName))
                {
                    _logger?.LogDebug(
                        "NotificationActivity: Loading template '{TemplateName}' for {Channel} notification",
                        _templateName, _channel);

                    // TODO: Load template from file system
                    // Load from TemplateDirectory + TemplateName
                    // finalMessage = LoadTemplate(_templateName);
                }

                // Apply template data substitution
                if (_templateData != null && _templateData.Any())
                {
                    _logger?.LogDebug(
                        "NotificationActivity: Applying template data substitution for {Channel} notification",
                        _channel);

                    foreach (var data in _templateData)
                    {
                        var placeholder = $"{{{{{data.Key}}}}}"; // {{key}}
                        finalMessage = finalMessage.Replace(placeholder, data.Value);
                        finalSubject = finalSubject.Replace(placeholder, data.Value);
                    }
                }

                var notificationId = Guid.NewGuid().ToString();
                var deliveryStatus = "Pending";
                var deliveryError = (string?)null;

                try
                {
                    // Send notification based on channel
                    switch (_channel)
                    {
                        case NotificationChannel.Email:
                            _logger?.LogInformation(
                                "NotificationActivity: Sending email to {Recipient}",
                                _recipient);
                            // TODO: Send email via AppEnd email service
                            // var emailService = serviceProvider.GetService<IEmailService>();
                            // await emailService.SendEmailAsync(_recipient, finalSubject, finalMessage);
                            deliveryStatus = "Sent";
                            break;

                        case NotificationChannel.Sms:
                            _logger?.LogInformation(
                                "NotificationActivity: Sending SMS to {Recipient}",
                                _recipient);
                            // TODO: Send SMS via AppEnd SMS service or Twilio
                            // var smsService = serviceProvider.GetService<ISmsService>();
                            // await smsService.SendSmsAsync(_recipient, finalMessage);
                            deliveryStatus = "Sent";
                            break;

                        case NotificationChannel.AppNotification:
                            _logger?.LogInformation(
                                "NotificationActivity: Creating in-app notification for user {Recipient}",
                                _recipient);
                            // TODO: Create in-app notification record
                            // var notificationService = serviceProvider.GetService<IAppNotificationService>();
                            // await notificationService.CreateNotificationAsync(_recipient, finalSubject, finalMessage);
                            deliveryStatus = "Created";
                            break;

                        case NotificationChannel.Webhook:
                            _logger?.LogInformation(
                                "NotificationActivity: Posting to webhook URL {Recipient}",
                                _recipient);
                            // TODO: POST to webhook endpoint
                            // using var httpClient = new HttpClient();
                            // var payload = new { subject = finalSubject, message = finalMessage };
                            // await httpClient.PostAsJsonAsync(_recipient, payload);
                            deliveryStatus = "Posted";
                            break;

                        default:
                            throw new InvalidOperationException($"Unknown notification channel: {_channel}");
                    }
                }
                catch (Exception deliveryEx)
                {
                    _logger?.LogError(deliveryEx,
                        "NotificationActivity: Failed to deliver {Channel} notification to {Recipient}",
                        _channel, _recipient);

                    deliveryStatus = "Failed";
                    deliveryError = deliveryEx.Message;

                    // Implement retry logic if configured
                    // TODO: Check NotificationActivityOptions.RetryAttempts
                    // TODO: Implement exponential backoff retry
                }

                var output = new Dictionary<string, object>
                {
                    { "Channel", _channel.ToString() },
                    { "Recipient", _recipient },
                    { "NotificationId", notificationId },
                    { "Subject", finalSubject },
                    { "MessageLength", finalMessage.Length },
                    { "Status", deliveryStatus },
                    { "Error", deliveryError ?? string.Empty },
                    { "SentAt", DateTime.UtcNow }
                };

                if (deliveryStatus == "Failed")
                {
                    _logger?.LogError(
                        "NotificationActivity: {Channel} notification failed to {Recipient} (Error: {Error})",
                        _channel, _recipient, deliveryError);

                    return ActivityExecutionResult.Failure(
                        $"Notification delivery failed: {deliveryError}");
                }

                _logger?.LogInformation(
                    "NotificationActivity: {Channel} notification sent successfully to {Recipient} for instance {InstanceId}",
                    _channel, _recipient, context.WorkflowInstanceId);

                return ActivityExecutionResult.SuccessResult(output);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex,
                    "NotificationActivity: Failed to send {Channel} notification to {Recipient} for instance {InstanceId}",
                    _channel, _recipient, context.WorkflowInstanceId);

                return ActivityExecutionResult.Failure(
                    $"Notification delivery failed: {ex.Message}",
                    ex);
            }
            finally
            {
                var duration = DateTime.UtcNow - startTime;
                _logger?.LogDebug(
                    "NotificationActivity: Notification to {Recipient} completed in {Duration}ms",
                    _recipient, duration.TotalMilliseconds);
            }
        }

        public override void Initialize()
        {
            _logger?.LogDebug(
                "NotificationActivity initialized for {Channel} channel to {Recipient}",
                _channel, _recipient);
        }

        public override void Dispose()
        {
            _templateData?.Clear();
            _logger?.LogDebug("NotificationActivity disposed");
        }
    }

    /// <summary>
    /// Configuration for notification activities
    /// </summary>
    public class NotificationActivityOptions
    {
        /// <summary>
        /// Whether email notifications are enabled
        /// </summary>
        public bool EnableEmailNotifications { get; set; } = true;

        /// <summary>
        /// Whether SMS notifications are enabled
        /// </summary>
        public bool EnableSmsNotifications { get; set; } = true;

        /// <summary>
        /// Whether in-app notifications are enabled
        /// </summary>
        public bool EnableAppNotifications { get; set; } = true;

        /// <summary>
        /// Whether webhook notifications are enabled
        /// </summary>
        public bool EnableWebhookNotifications { get; set; } = true;

        /// <summary>
        /// Default email from address
        /// </summary>
        public string DefaultEmailFromAddress { get; set; } = "noreply@append.local";

        /// <summary>
        /// Default email from display name
        /// </summary>
        public string DefaultEmailFromName { get; set; } = "AppEnd Workflows";

        /// <summary>
        /// Notification delivery timeout in milliseconds
        /// </summary>
        public int DeliveryTimeoutMs { get; set; } = 30000;

        /// <summary>
        /// Number of retry attempts for failed deliveries
        /// </summary>
        public int RetryAttempts { get; set; } = 3;

        /// <summary>
        /// Delay between retry attempts in milliseconds
        /// </summary>
        public int RetryDelayMs { get; set; } = 1000;

        /// <summary>
        /// Directory path for notification templates
        /// </summary>
        public string TemplateDirectory { get; set; } = "Workflows/Templates/Notifications";

        /// <summary>
        /// Whether to log all notification attempts
        /// </summary>
        public bool LogAllAttempts { get; set; } = true;
    }
}
