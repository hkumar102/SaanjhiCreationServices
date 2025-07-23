using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.ErrorHandling;
using Microsoft.Extensions.Logging;
using RentalService.Contracts.Enums;
using RentalService.Infrastructure.Persistence;
using RentalService.Infrastructure.HttpClients;
using RentalService.Contracts.DTOs;

namespace RentalService.Application.Rentals.Commands.UpdateStatus;

public class UpdateRentalStatusCommandHandler : IRequestHandler<UpdateRentalStatusCommand>
{
    private readonly RentalDbContext dbContext;
    private readonly ILogger<UpdateRentalStatusCommandHandler> logger;
    private readonly IProductApiClient productApiClient;

    public UpdateRentalStatusCommandHandler(
        RentalDbContext dbContext,
        IProductApiClient productApiClient,
        ILogger<UpdateRentalStatusCommandHandler> logger)
    {
        this.dbContext = dbContext;
        this.productApiClient = productApiClient;
        this.logger = logger;
    }

    public async Task Handle(UpdateRentalStatusCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug("UpdateRentalStatusCommand received for RentalId: {RentalId} to Status: {Status}", request.Id, request.Status);
        var entity = await dbContext.Rentals.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (entity == null)
        {
            logger.LogDebug("Rental not found for Id: {RentalId}", request.Id);
            throw new KeyNotFoundException("Rental not found.");
        }

        if (!IsValidTransition(entity.Status, request.Status))
        {
            logger.LogDebug("Invalid status transition from {CurrentStatus} to {NextStatus} for RentalId: {RentalId}", entity.Status, request.Status, request.Id);
            throw new BusinessRuleException($"Cannot transition from {entity.Status} to {request.Status}.");
        }

        var inventoryItem = await productApiClient.GetInventoryByIdAsync(entity.InventoryItemId, cancellationToken);

        if (inventoryItem == null)
        {
            logger.LogError("Inventory item with ID {InventoryItemId} not found", entity.InventoryItemId);
            throw new BusinessRuleException($"Inventory item with ID {entity.InventoryItemId} not found");
        }

        if (request.Status == RentalStatus.Booked)
        {
            if (inventoryItem.Status != InventoryStatus.Available)
            {
                logger.LogError("Inventory item with ID {InventoryItemId} is not available for booking", entity.InventoryItemId);
                throw new BusinessRuleException($"Inventory item with ID {entity.InventoryItemId} is not available for booking");
            }

            // Update inventory item status to Rented
            await productApiClient.UpdateInventoryItemStatusAsync(entity.InventoryItemId, InventoryStatus.Rented, $"Inventory item rented for {request.Id}, because it was booked", cancellationToken);
        }
        else if (request.Status == RentalStatus.Cancelled)
        {
            // Update inventory item status to Available
            await productApiClient.UpdateInventoryItemStatusAsync(entity.InventoryItemId, InventoryStatus.Available, $"Inventory item returned from {request.Id}, because it was cancelled", cancellationToken);
        }
        else if (request.Status == RentalStatus.Returned)
        {
            if (!request.ActualReturnDate.HasValue)
                throw new BusinessRuleException("ActualReturnDate is required when marking as Returned.");
            entity.ActualReturnDate = request.ActualReturnDate.Value;
            await productApiClient.UpdateInventoryItemStatusAsync(entity.InventoryItemId, InventoryStatus.Available, $"Inventory item returned from {request.Id}, because it was returned", cancellationToken);
        }


        entity.Status = request.Status;
        if (!string.IsNullOrWhiteSpace(request.Notes))
            entity.Notes = request.Notes;

        if (request.Status == RentalStatus.PickedUp)
        {
            if (!request.ActualStartDate.HasValue)
                throw new BusinessRuleException("ActualStartDate is required when marking as PickedUp.");
            entity.ActualStartDate = request.ActualStartDate.Value;
        }

        // Add RentalTimeline entry for status change
        dbContext.RentalTimelines.Add(new RentalService.Domain.Entities.RentalTimeline
        {
            Id = Guid.NewGuid(),
            RentalId = entity.Id,
            Status = (int)request.Status,
            Notes = request.Notes
        });

        // we need to handle the exception here and if exeption in creating rental, we need to rollback the inventory item status
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogDebug("Rental status updated for RentalId: {RentalId} to Status: {Status}", request.Id, request.Status);

        }
        catch (Exception ex)
        {
            if (request.Status == RentalStatus.Booked)
            {
                await productApiClient.UpdateInventoryItemStatusAsync(entity.InventoryItemId, InventoryStatus.Available, request.Notes, cancellationToken);
            }
            logger.LogError(ex, "Error occurred while updating rental status for RentalId: {RentalId}", request.Id);
            throw ex;
        }
    }

    private static bool IsValidTransition(RentalStatus current, RentalStatus next)
    {
        return current switch
        {
            RentalStatus.Pending => next == RentalStatus.Booked || next == RentalStatus.Cancelled,
            RentalStatus.Booked => next == RentalStatus.PickedUp || next == RentalStatus.Cancelled,
            RentalStatus.PickedUp => next == RentalStatus.Returned || next == RentalStatus.Overdue,
            RentalStatus.Overdue => next == RentalStatus.Returned,
            RentalStatus.Returned => false,
            RentalStatus.Cancelled => false,
            _ => false
        };
    }
}
