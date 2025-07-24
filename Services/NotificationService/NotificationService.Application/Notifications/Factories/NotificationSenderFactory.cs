using Microsoft.Extensions.Logging;
using NotificationService.Contracts.DTOs;
using NotificationService.Domain;
using NotificationService.Contracts.Enums;

namespace NotificationService.Application.Notifications.Factories;

public class NotificationSenderFactory
{
    private readonly NotificationConfig _config;
    private readonly ILogger<NotificationSenderFactory> _logger;
    private readonly IDictionary<NotificationChannel, INotificationSender> _senders;

    public NotificationSenderFactory(
        NotificationConfig config,
        ILogger<NotificationSenderFactory> logger,
        PushNotificationSender pushSender)
    {
        _config = config;
        _logger = logger;
        _senders = new Dictionary<NotificationChannel, INotificationSender>
        {
            { NotificationChannel.Push, pushSender }
            // Add other senders here (SMS, Email)
        };
    }

    public INotificationSender? GetSender(NotificationChannel channel)
    {
        _logger.LogTrace("Resolving sender for channel: {Channel}", channel);
        return channel switch
        {
            NotificationChannel.Push when _config.IsPushEnabled => _senders[NotificationChannel.Push],
            NotificationChannel.SMS when _config.IsSmsEnabled => _senders.TryGetValue(NotificationChannel.SMS, out var smsSender) ? smsSender : null,
            NotificationChannel.Email when _config.IsEmailEnabled => _senders.TryGetValue(NotificationChannel.Email, out var emailSender) ? emailSender : null,
            _ => null
        };
    }
}
