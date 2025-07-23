using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Inventory.Commands.UpdateInventoryItemStatus;

public class UpdateInventoryItemStatusCommandHandler(
    ProductDbContext db, 
    IMapper mapper,
    ILogger<UpdateInventoryItemStatusCommandHandler> logger)
    : IRequestHandler<UpdateInventoryItemStatusCommand, InventoryItemDto>
{
    public async Task<InventoryItemDto> Handle(UpdateInventoryItemStatusCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting UpdateInventoryItemStatusCommand execution for InventoryItemId: {InventoryItemId}", request.InventoryItemId);

        try
        {
            var inventoryItem = await db.InventoryItems
                .FirstOrDefaultAsync(i => i.Id == request.InventoryItemId, cancellationToken);

            if (inventoryItem == null)
            {
                throw new KeyNotFoundException($"Inventory item with ID {request.InventoryItemId} not found");
            }

            var oldStatus = inventoryItem.Status;
            inventoryItem.Status = request.Status;
            inventoryItem.ModifiedAt = DateTime.UtcNow;

            // Add notes if provided
            if (!string.IsNullOrWhiteSpace(request.Notes))
            {
                var updatedNotes = string.IsNullOrWhiteSpace(inventoryItem.ConditionNotes) 
                    ? request.Notes 
                    : $"{inventoryItem.ConditionNotes}\n[{DateTime.UtcNow:yyyy-MM-dd HH:mm}] {request.Notes}";
                inventoryItem.ConditionNotes = updatedNotes;
            }

            // Handle specific status transitions
            switch (request.Status)
            {
                case Contracts.Enums.InventoryStatus.Rented:
                    inventoryItem.LastRentedDate = DateTime.UtcNow;
                    inventoryItem.TimesRented++;
                    break;
                case Contracts.Enums.InventoryStatus.Available when oldStatus == Contracts.Enums.InventoryStatus.Rented:
                    // Update last rented date when returning from rental
                    inventoryItem.LastMaintenanceDate = DateTime.UtcNow;
                    break;
                case Contracts.Enums.InventoryStatus.Damaged:
                case Contracts.Enums.InventoryStatus.Retired:
                    inventoryItem.IsRetired = true;
                    inventoryItem.RetirementDate = DateTime.UtcNow;
                    inventoryItem.RetirementReason = request.Status.ToString();
                    break;
            }

            await db.SaveChangesAsync(cancellationToken);

            logger.LogDebug("Successfully updated inventory item status from {OldStatus} to {NewStatus}", 
                oldStatus, request.Status);

            var inventoryItemDto = mapper.Map<InventoryItemDto>(inventoryItem);
            return inventoryItemDto;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing UpdateInventoryItemStatusCommand for InventoryItemId: {InventoryItemId}", 
                request.InventoryItemId);
            throw;
        }
    }
}
