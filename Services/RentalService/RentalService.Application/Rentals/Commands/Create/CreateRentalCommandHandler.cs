using RentalService.Contracts.Enums;
using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotificationService.Contracts.Enums;
using RentalService.Application.Notifications.Commands;
using RentalService.Application.Rentals.Commands.Create;
using RentalService.Application.Rentals.Queries.GetRentalById;
using RentalService.Contracts.DTOs;
using RentalService.Domain.Entities;
using RentalService.Infrastructure.Persistence;
using RentalService.Infrastructure.HttpClients;
using Shared.ErrorHandling;

public class CreateRentalCommandHandler(RentalDbContext dbContext, IMapper mapper, IProductApiClient productApiClient, ILogger<CreateRentalCommandHandler> logger, IMediator mediator)
    : IRequestHandler<CreateRentalCommand, Guid>
{
    public async Task<Guid> Handle(CreateRentalCommand request, CancellationToken cancellationToken)
    {
        //before we create a rental, we need to ensure that the inventory item is available
        // which can come from ProductApiClient, we need to inject IProductApiClient

        var inventoryItem = await productApiClient.GetInventoryByIdAsync(request.InventoryItemId, cancellationToken);

        if (inventoryItem == null)
        {
            logger.LogError("Inventory item with ID {InventoryItemId} not found", request.InventoryItemId);
            throw new BusinessRuleException($"Inventory item with ID {request.InventoryItemId} not found");
        }

        if (inventoryItem.Status != InventoryStatus.Available)
        {
            logger.LogError("Inventory item with ID {InventoryItemId} is not available for rental", request.InventoryItemId);
            throw new BusinessRuleException($"Inventory item with ID {request.InventoryItemId} is not available for rental");
        }


        logger.LogDebug("CreateRentalCommand received for ProductId: {ProductId}, InventoryItemId: {InventoryItemId}, CustomerId: {CustomerId}", request.ProductId, request.InventoryItemId, request.CustomerId);
        var entity = mapper.Map<Rental>(request);
        entity.Status = RentalStatus.Pending;

        // Generate user-friendly RentalNumber: RNT-YYYYMMDD-XXXXX
        var today = DateTime.UtcNow.Date;
        var countToday = await dbContext.Rentals.CountAsync(r => r.CreatedAt.Date == today, cancellationToken);
        entity.RentalNumber = $"RNT-{today:yyyyMMdd}-{countToday + 1:D5}";

        logger.LogDebug("Rental entity mapped, status set to Pending, and RentalNumber generated: {RentalNumber}", entity.RentalNumber);
        dbContext.Rentals.Add(entity);

        dbContext.RentalTimelines.Add(new RentalTimeline
        {
            RentalId = entity.Id,
            Status = (int)entity.Status,
            Notes = request.Notes,
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogDebug("DbContext changes saved for new RentalId: {RentalId}", entity.Id);


        var rentalDto = await mediator.Send(new GetRentalByIdQuery() { Id = entity.Id }, cancellationToken);
        // Send notification after rental creation
        var notificationCommand = new SendRentalNotificationCommand
        {
            Rental = rentalDto,
            Type = NotificationType.RentalCreated
        };

        await mediator.Send(notificationCommand, cancellationToken);
        return entity.Id;
    }
}