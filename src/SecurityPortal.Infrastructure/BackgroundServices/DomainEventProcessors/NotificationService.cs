using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using SecurityPortal.Infrastructure.BackgroundServices.Common;
using System.Collections.Concurrent;

namespace SecurityPortal.Infrastructure.BackgroundServices.DomainEventProcessors;

/// <summary>
/// Centralized notification service for handling email, SMS, and real-time notifications
/// </summary>
public class NotificationService : BaseBackgroundService
{
    private readonly ConcurrentQueue<NotificationRequest> _notificationQueue = new();
    private readonly NotificationOptions _options;

    public NotificationService(
        ILogger<NotificationService> logger,
        IServiceProvider serviceProvider,
        NotificationOptions? options = null) 
        : base(logger, serviceProvider)
    {
        _options = options ?? new NotificationOptions();
    }

    protected override async Task ExecuteServiceAsync(CancellationToken stoppingToken)
    {
        Logger.LogInformation("Notification Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                UpdateLastActivity();
                
                // Process queued notifications
                await ProcessNotificationQueue(stoppingToken);
                
                // Wait before checking queue again
                await Task.Delay(_options.QueueProcessingInterval, stoppingToken);
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                HandleError(ex, "ProcessingNotifications");
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }

    /// <summary>
    /// Queues a notification for processing
    /// </summary>
    public void QueueNotification(NotificationRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        
        _notificationQueue.Enqueue(request);
        Logger.LogDebug("Queued {NotificationType} notification: {Subject}", 
            request.Type, request.Subject);
    }

    private async Task ProcessNotificationQueue(CancellationToken cancellationToken)
    {
        var processedCount = 0;
        var maxBatchSize = _options.MaxBatchSize;

        while (processedCount < maxBatchSize && 
               _notificationQueue.TryDequeue(out var notification))
        {
            try
            {
                await ProcessSingleNotification(notification, cancellationToken);
                processedCount++;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, 
                    "Error processing {NotificationType} notification: {Subject}",
                    notification.Type, notification.Subject);
                
                // Optionally re-queue failed notifications for retry
                if (notification.RetryCount < _options.MaxRetries)
                {
                    notification.RetryCount++;
                    notification.NextRetryAt = DateTime.UtcNow.Add(_options.RetryDelay);
                    _notificationQueue.Enqueue(notification);
                }
            }
        }

        if (processedCount > 0)
        {
            Logger.LogDebug("Processed {ProcessedCount} notifications", processedCount);
        }
    }

    private async Task ProcessSingleNotification(NotificationRequest notification, CancellationToken cancellationToken)
    {
        // Check if it's time to process this notification (for retries)
        if (notification.NextRetryAt.HasValue && 
            DateTime.UtcNow < notification.NextRetryAt.Value)
        {
            // Re-queue for later processing
            _notificationQueue.Enqueue(notification);
            return;
        }

        using var scope = CreateScope();
        
        switch (notification.Type)
        {
            case NotificationType.Email:
                await SendEmailNotification(notification, scope, cancellationToken);
                break;
                
            case NotificationType.Sms:
                await SendSmsNotification(notification, scope, cancellationToken);
                break;
                
            case NotificationType.Push:
                await SendPushNotification(notification, scope, cancellationToken);
                break;
                
            case NotificationType.SignalR:
                await SendSignalRNotification(notification, scope, cancellationToken);
                break;
                
            default:
                Logger.LogWarning("Unknown notification type: {NotificationType}", notification.Type);
                break;
        }
    }

    private async Task SendEmailNotification(NotificationRequest notification, IServiceScope scope, CancellationToken cancellationToken)
    {
        Logger.LogDebug("Sending email notification to {Recipients}", 
            string.Join(", ", notification.Recipients));
        
        // Implementation would use email service (e.g., SendGrid, SMTP)
        // For now, just log the notification
        
        Logger.LogInformation("Email sent - Subject: {Subject}, Recipients: {Recipients}", 
            notification.Subject, string.Join(", ", notification.Recipients));
        
        await Task.CompletedTask;
    }

    private async Task SendSmsNotification(NotificationRequest notification, IServiceScope scope, CancellationToken cancellationToken)
    {
        Logger.LogDebug("Sending SMS notification to {Recipients}", 
            string.Join(", ", notification.Recipients));
        
        // Implementation would use SMS service (e.g., Twilio, AWS SNS)
        
        Logger.LogInformation("SMS sent - Message: {Message}, Recipients: {Recipients}", 
            notification.Message, string.Join(", ", notification.Recipients));
        
        await Task.CompletedTask;
    }

    private async Task SendPushNotification(NotificationRequest notification, IServiceScope scope, CancellationToken cancellationToken)
    {
        Logger.LogDebug("Sending push notification to {Recipients}", 
            string.Join(", ", notification.Recipients));
        
        // Implementation would use push notification service (e.g., Firebase, Azure Notification Hubs)
        
        Logger.LogInformation("Push notification sent - Title: {Subject}, Recipients: {Recipients}", 
            notification.Subject, string.Join(", ", notification.Recipients));
        
        await Task.CompletedTask;
    }

    private async Task SendSignalRNotification(NotificationRequest notification, IServiceScope scope, CancellationToken cancellationToken)
    {
        Logger.LogDebug("Sending SignalR notification to {Recipients}", 
            string.Join(", ", notification.Recipients));
        
        // Implementation would use SignalR hub context
        
        Logger.LogInformation("SignalR notification sent - Method: {Method}, Recipients: {Recipients}", 
            notification.SignalRMethod ?? "DefaultMethod", string.Join(", ", notification.Recipients));
        
        await Task.CompletedTask;
    }
}

/// <summary>
/// Represents a notification request
/// </summary>
public class NotificationRequest
{
    public NotificationType Type { get; set; }
    public List<string> Recipients { get; set; } = new();
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
    public Dictionary<string, object> Data { get; set; } = new();
    
    // For SignalR notifications
    public string? SignalRMethod { get; set; }
    public string? SignalRGroup { get; set; }
    
    // For retry logic
    public int RetryCount { get; set; } = 0;
    public DateTime? NextRetryAt { get; set; }
    
    // Tracking
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CorrelationId { get; set; }
    
    // Template support
    public string? TemplateName { get; set; }
    public Dictionary<string, object> TemplateData { get; set; } = new();
}

/// <summary>
/// Types of notifications
/// </summary>
public enum NotificationType
{
    Email,
    Sms,
    Push,
    SignalR
}

/// <summary>
/// Notification priority levels
/// </summary>
public enum NotificationPriority
{
    Low,
    Normal,
    High,
    Critical
}

/// <summary>
/// Configuration options for the notification service
/// </summary>
public class NotificationOptions
{
    /// <summary>
    /// How often to check the notification queue
    /// </summary>
    public TimeSpan QueueProcessingInterval { get; set; } = TimeSpan.FromSeconds(10);
    
    /// <summary>
    /// Maximum number of notifications to process in one batch
    /// </summary>
    public int MaxBatchSize { get; set; } = 50;
    
    /// <summary>
    /// Maximum retry attempts for failed notifications
    /// </summary>
    public int MaxRetries { get; set; } = 3;
    
    /// <summary>
    /// Delay between retry attempts
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMinutes(5);
    
    /// <summary>
    /// Email service configuration
    /// </summary>
    public EmailOptions Email { get; set; } = new();
    
    /// <summary>
    /// SMS service configuration
    /// </summary>
    public SmsOptions Sms { get; set; } = new();
}

/// <summary>
/// Email service configuration
/// </summary>
public class EmailOptions
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool UseSSL { get; set; } = true;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = "Security Portal";
}

/// <summary>
/// SMS service configuration
/// </summary>
public class SmsOptions
{
    public string Provider { get; set; } = "Twilio"; // Twilio, AWS SNS, etc.
    public string AccountSid { get; set; } = string.Empty;
    public string AuthToken { get; set; } = string.Empty;
    public string FromNumber { get; set; } = string.Empty;
}

/// <summary>
/// Helper class for creating common notification types
/// </summary>
public static class NotificationFactory
{
    public static NotificationRequest CreateIncidentNotification(
        Guid incidentId, 
        string title, 
        string priority,
        List<string> recipients)
    {
        return new NotificationRequest
        {
            Type = NotificationType.Email,
            Recipients = recipients,
            Subject = $"Security Incident Created: {title}",
            Message = $"A new security incident has been created with priority {priority}.",
            Priority = GetNotificationPriority(priority),
            Data = new Dictionary<string, object>
            {
                ["IncidentId"] = incidentId,
                ["Title"] = title,
                ["Priority"] = priority
            },
            TemplateName = "incident-created",
            TemplateData = new Dictionary<string, object>
            {
                ["IncidentId"] = incidentId,
                ["Title"] = title,
                ["Priority"] = priority
            }
        };
    }

    public static NotificationRequest CreateAlertNotification(
        Guid alertId,
        string title,
        string priority,
        Guid plantId,
        List<string> recipients)
    {
        return new NotificationRequest
        {
            Type = priority == "Critical" ? NotificationType.Sms : NotificationType.Email,
            Recipients = recipients,
            Subject = $"Security Alert: {title}",
            Message = $"Security alert triggered with priority {priority}.",
            Priority = GetNotificationPriority(priority),
            Data = new Dictionary<string, object>
            {
                ["AlertId"] = alertId,
                ["Title"] = title,
                ["Priority"] = priority,
                ["PlantId"] = plantId
            },
            TemplateName = "alert-triggered",
            TemplateData = new Dictionary<string, object>
            {
                ["AlertId"] = alertId,
                ["Title"] = title,
                ["Priority"] = priority,
                ["PlantId"] = plantId
            }
        };
    }

    public static NotificationRequest CreateSignalRNotification(
        string method,
        string group,
        object data)
    {
        return new NotificationRequest
        {
            Type = NotificationType.SignalR,
            SignalRMethod = method,
            SignalRGroup = group,
            Priority = NotificationPriority.High,
            Data = new Dictionary<string, object> { ["payload"] = data }
        };
    }

    private static NotificationPriority GetNotificationPriority(string priority)
    {
        return priority.ToLowerInvariant() switch
        {
            "critical" => NotificationPriority.Critical,
            "high" => NotificationPriority.High,
            "medium" => NotificationPriority.Normal,
            "low" => NotificationPriority.Low,
            _ => NotificationPriority.Normal
        };
    }
}