using MediatR;
using NotificationService.Contracts.Enums;
using RentalService.Contracts.DTOs;

namespace RentalService.Application.Notifications.Commands;

public class SendRentalNotificationCommand : IRequest
{
    public object Rental { get; set; }
    public NotificationType Type { get; set; } = NotificationType.RentalCreated; // e.g. RentalCreated, RentalStatusChanged
    // Optionally, add additional context fields if needed in the future
}
