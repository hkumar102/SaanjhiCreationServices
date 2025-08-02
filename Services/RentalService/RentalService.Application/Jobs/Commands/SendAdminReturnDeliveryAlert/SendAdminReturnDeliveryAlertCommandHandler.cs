using AutoMapper;
using RentalService.Infrastructure.Persistence;
using RentalService.Domain.Entities;
using RentalService.Contracts.Enums;
using MediatR;
using Microsoft.Extensions.Configuration;
using RentalService.Application.Notifications.Commands;
using Microsoft.Extensions.Logging;
using NotificationService.Contracts.Enums;
using RentalService.Contracts.DTOs;
using RentalService.Application.Rentals.Queries.GetRentalsWithDetails;

namespace RentalService.Application.Jobs.Commands.SendAdminReturnDeliveryAlert;

public class SendAdminReturnDeliveryAlertCommandHandler : IRequestHandler<SendAdminReturnDeliveryAlertCommand>
{

    private readonly ILogger<SendAdminReturnDeliveryAlertCommandHandler> _logger;
    private readonly RentalDbContext _dbContext;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly string _baseUrl;

    public SendAdminReturnDeliveryAlertCommandHandler(
        ILogger<SendAdminReturnDeliveryAlertCommandHandler> logger,
        RentalDbContext dbContext,
        IMediator mediator,
        IMapper mapper,
        IConfiguration configuration)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mediator = mediator;
        _mapper = mapper;
        _baseUrl = configuration["App:AdminPortalBaseUrl"] ?? "https://www.saanjhicreation.com";
    }

    public async Task Handle(SendAdminReturnDeliveryAlertCommand request, CancellationToken cancellationToken)
    {
        using var scope = _logger.BeginScope("SendAdminReturnDeliveryAlertJob");
        _logger.LogTrace("Starting admin return/delivery alert notification job");
        var today = DateTime.UtcNow.Date;
        var dueReturnsQueryable = _dbContext.Rentals
            .Where(r => r.EndDate.Date == today && r.Status == RentalStatus.PickedUp);
        var dueDeliveriesQueryable = _dbContext.Rentals
            .Where(r => r.StartDate.Date == today && r.Status == RentalStatus.Booked);

        var dueReturns = await _mediator.Send(new GetRentalsWithDetailsQuery() { Queryable = dueReturnsQueryable });
        var dueDeliveries = await _mediator.Send(new GetRentalsWithDetailsQuery() { Queryable = dueDeliveriesQueryable });
        
        _logger.LogInformation("Found {ReturnCount} returns and {DeliveryCount} deliveries due today", dueReturns.Count, dueDeliveries.Count);

        foreach (var rental in dueReturns)
        {
            _logger.LogTrace("Sending admin alert for return due: {RentalNumber} (Id: {RentalId})", rental.RentalNumber, rental.Id);
            await _mediator.Send(new SendRentalNotificationCommand
            {
                //(#{RentalNumber}) for {CustomerName} ({CustomerPhone}) is due for return today. Product: {ProductName} | Category: {CategoryName} | Size: {Size} | Color: {Color}. [View Details]({RentalLink})
                Rental = new {
                    rental.RentalNumber,
                    CustomerName = rental.Customer?.Name,
                    CustomerPhone = rental.Customer?.PhoneNumber,
                    ProductName = rental.Product?.Name,
                    rental.Product.CategoryName,
                    rental.InventoryItem?.Size,
                    rental.InventoryItem?.Color,
                    Link = $"{_baseUrl.TrimEnd('/')}/rentals/manage/{rental.Id}"
                },
                Type = NotificationType.ReturnReminder
            }, cancellationToken);
        }
        foreach (var rental in dueDeliveries)
        {
            _logger.LogTrace("Sending admin alert for delivery due: {RentalNumber} (Id: {RentalId})", rental.RentalNumber, rental.Id);
            await _mediator.Send(new SendRentalNotificationCommand
            {
                Rental = new {
                    rental.RentalNumber,
                    CustomerName = rental.Customer?.Name,
                    CustomerPhone = rental.Customer?.PhoneNumber,
                    ProductName = rental.Product?.Name,
                    rental.Product?.CategoryName,
                    rental.InventoryItem?.Size,
                    rental.InventoryItem?.Color,
                Link = $"{_baseUrl.TrimEnd('/')}/rentals/details/{rental.Id}"
                },
                Type = NotificationType.DeliveryReminder
            }, cancellationToken);
        }
        _logger.LogInformation("Admin return/delivery alert notification job completed. {Total} alerts sent.", dueReturns.Count + dueDeliveries.Count);
    }
}
