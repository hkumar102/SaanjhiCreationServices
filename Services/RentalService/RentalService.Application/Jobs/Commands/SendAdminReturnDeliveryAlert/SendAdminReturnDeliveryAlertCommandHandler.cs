using AutoMapper;
using RentalService.Infrastructure.Persistence;
using RentalService.Domain.Entities;
using RentalService.Contracts.Enums;
using MediatR;
using RentalService.Application.Notifications.Commands;
using Microsoft.Extensions.Logging;
using NotificationService.Contracts.Enums;
using RentalService.Contracts.DTOs;

namespace RentalService.Application.Jobs.Commands.SendAdminReturnDeliveryAlert;

public class SendAdminReturnDeliveryAlertCommandHandler : IRequestHandler<SendAdminReturnDeliveryAlertCommand>
{
    private readonly ILogger<SendAdminReturnDeliveryAlertCommandHandler> _logger;
    private readonly RentalDbContext _dbContext;
    private readonly IMediator _mediator;

    private readonly IMapper _mapper;

    public SendAdminReturnDeliveryAlertCommandHandler(
        ILogger<SendAdminReturnDeliveryAlertCommandHandler> logger,
        RentalDbContext dbContext,
        IMediator mediator,
        IMapper mapper)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task Handle(SendAdminReturnDeliveryAlertCommand request, CancellationToken cancellationToken)
    {
        using var scope = _logger.BeginScope("SendAdminReturnDeliveryAlertJob");
        _logger.LogTrace("Starting admin return/delivery alert notification job");
        var today = DateTime.UtcNow.Date;
        var dueReturns = _dbContext.Rentals
            .Where(r => r.EndDate.Date == today && r.Status != RentalStatus.Returned && r.Status != RentalStatus.Cancelled)
            .ToList();
        var dueDeliveries = _dbContext.Rentals
            .Where(r => r.StartDate.Date == today && r.Status == RentalStatus.Booked)
            .ToList();
        _logger.LogInformation("Found {ReturnCount} returns and {DeliveryCount} deliveries due today", dueReturns.Count, dueDeliveries.Count);
        foreach (var rental in dueReturns)
        {
            _logger.LogTrace("Sending admin alert for return due: {RentalNumber} (Id: {RentalId})", rental.RentalNumber, rental.Id);
            await _mediator.Send(new SendRentalNotificationCommand
            {
                Rental = _mapper.Map<RentalDto>(rental),
                Type = NotificationType.RentalStatusChanged // Or a custom type if you have one for admin alerts
            }, cancellationToken);
        }
        foreach (var rental in dueDeliveries)
        {
            _logger.LogTrace("Sending admin alert for delivery due: {RentalNumber} (Id: {RentalId})", rental.RentalNumber, rental.Id);
            await _mediator.Send(new SendRentalNotificationCommand
            {
                Rental = _mapper.Map<RentalDto>(rental),
                Type = NotificationType.RentalStatusChanged // Or a custom type if you have one for admin alerts
            }, cancellationToken);
        }
        _logger.LogInformation("Admin return/delivery alert notification job completed. {Total} alerts sent.", dueReturns.Count + dueDeliveries.Count);
    }
}
