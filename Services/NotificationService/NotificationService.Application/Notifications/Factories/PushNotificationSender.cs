using Microsoft.Extensions.Logging;
using NotificationService.Domain;
using NotificationService.Contracts.Enums;
using NotificationService.Infrastructure.Pushover;

namespace NotificationService.Application.Notifications.Factories;

public class PushNotificationSender : INotificationSender
{
    private readonly ILogger<PushNotificationSender> _logger;
    private readonly PushoverClient _pushoverClient;

    public PushNotificationSender(ILogger<PushNotificationSender> logger, PushoverClient pushoverClient)
    {
        _logger = logger;
        _pushoverClient = pushoverClient;
    }

    public async Task<NotificationStatus> SendAsync(Notification notification, CancellationToken cancellationToken)
    {
        _logger.LogDebug("[Push] Sending notification: {@Notification}", notification);
        try
        {
            await _pushoverClient.SendMessageAsync(notification, cancellationToken);
            _logger.LogInformation("[Push] Notification sent to user {RecipientUserId}", notification.RecipientUserId);
            return NotificationStatus.Sent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Push] Failed to send notification to user {RecipientUserId}", notification.RecipientUserId);
            return NotificationStatus.Failed;
        }
    }
}
