using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Notifications.Commands;
using Microsoft.EntityFrameworkCore;

namespace RentalService.Application.Notifications.Commands;

using NotificationService.Infrastructure.Persistence;
// ...existing code...
public class SendRentalNotificationCommandHandler : IRequestHandler<SendRentalNotificationCommand>
{
    private readonly ILogger<SendRentalNotificationCommandHandler> _logger;
    private readonly IMediator _mediator;
    private readonly NotificationDbContext _notificationDbContext;

    public SendRentalNotificationCommandHandler(
        ILogger<SendRentalNotificationCommandHandler> logger,
        IMediator mediator,
        NotificationDbContext notificationDbContext)
    {
        _logger = logger;
        _mediator = mediator;
        _notificationDbContext = notificationDbContext;
    }

    public async Task Handle(SendRentalNotificationCommand request, CancellationToken cancellationToken)
    {
        var rental = request.Rental;
        if (rental == null)
        {
            _logger.LogWarning("Rental object is null in SendRentalNotificationCommand");
            return;
        }

        // Get the notification template from the database
        var notificationTemplate = await _notificationDbContext.NotificationTemplates
            .FirstOrDefaultAsync(t => t.Type == request.Type, cancellationToken);
        if (notificationTemplate == null)
        {
            _logger.LogWarning("No notification template found for type {Type}", request.Type);
            return;
        }

        // Extract fields for notification
        var rentalNumber = rental.RentalNumber;
        var customerName = rental.Customer?.Name ?? "";
        var customerPhone = rental.Customer?.PhoneNumber ?? "";
        var productName = rental.Product?.Name ?? "";
        var categoryName = rental.Product?.CategoryName ?? "";
        var size = rental.InventoryItem.Size;
        var color = rental.InventoryItem.Color;
        var status = rental.Status.ToString();
        var startDate = rental.StartDate;
        var endDate = rental.EndDate;

        _logger.LogInformation("Sending notification for rental process. Type: {Type}, RentalNumber: {RentalNumber}", request.Type, rentalNumber);

        // Select template based on notification type
        string titleTemplate = notificationTemplate.TitleTemplate;
        string messageTemplate = notificationTemplate.MessageTemplate;


        // Parse template with data
        string ParseTemplate(string template) => template
            .Replace("{RentalNumber}", rentalNumber)
            .Replace("{CustomerName}", customerName)
            .Replace("{CustomerPhone}", customerPhone)
            .Replace("{ProductName}", productName)
            .Replace("{CategoryName}", categoryName)
            .Replace("{Size}", size)
            .Replace("{Color}", color)
            .Replace("{Status}", status)
            .Replace("{StartDate}", startDate.ToString("d"))
            .Replace("{EndDate}", endDate.ToString("d"));

        var notificationCommand = new SendPushNotificationCommand
        {
            Type = request.Type,
            Title = ParseTemplate(titleTemplate),
            Message = ParseTemplate(messageTemplate),
            Metadata = System.Text.Json.JsonSerializer.Serialize(new { link = $"https://saanjhicreation.com/rentals/manage/{rental.Id}" })
        };

        await _mediator.Send(notificationCommand, cancellationToken);
    }
}
