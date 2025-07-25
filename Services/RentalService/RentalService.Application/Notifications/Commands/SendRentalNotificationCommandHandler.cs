using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Notifications.Commands;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;

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

        _logger.LogInformation("Sending notification for rental process. Type: {Type}, Metadata: {RentalNumber}", request.Type, rental.ToJson());

        // Select template based on notification type
        string titleTemplate = notificationTemplate.TitleTemplate.ParseTemplateWithObject(rental);
        string messageTemplate = notificationTemplate.MessageTemplate.ParseTemplateWithObject(rental);

        var notificationCommand = new SendPushNotificationCommand
        {
            Type = request.Type,
            Title = titleTemplate,
            Message = messageTemplate,
            Metadata = rental
        };

        await _mediator.Send(notificationCommand, cancellationToken);
    }
}
