using NotificationService.Domain;
using NotificationService.Contracts.Enums;

namespace NotificationService.Application.Notifications.Factories;

public interface INotificationSender
{
    Task<NotificationStatus> SendAsync(Notification notification, CancellationToken cancellationToken);
}
