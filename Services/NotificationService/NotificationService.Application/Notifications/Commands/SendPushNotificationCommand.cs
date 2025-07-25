using MediatR;
using NotificationService.Contracts.DTOs;
using NotificationService.Contracts.Enums;

namespace NotificationService.Application.Notifications.Commands;

public class SendPushNotificationCommand : IRequest<NotificationDto>
{
    public Guid RecipientUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public object? Metadata { get; set; }
    public NotificationType Type { get; set; }
}
