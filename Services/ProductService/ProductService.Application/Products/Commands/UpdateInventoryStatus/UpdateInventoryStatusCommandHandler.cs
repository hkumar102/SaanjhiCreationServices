using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Products.Commands.UpdateInventoryStatus;

public class UpdateInventoryStatusCommandHandler(
    ProductDbContext db, 
    IMapper mapper,
    ILogger<UpdateInventoryStatusCommandHandler> logger)
    : IRequestHandler<UpdateInventoryStatusCommand, InventoryItemDto>
{
    public async Task<InventoryItemDto> Handle(UpdateInventoryStatusCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting UpdateInventoryStatusCommand execution for InventoryItemId: {InventoryItemId}", request.InventoryItemId);

        try
        {
            var inventoryItem = await db.Products
                .SelectMany(p => p.InventoryItems)
                .FirstOrDefaultAsync(i => i.Id == request.InventoryItemId, cancellationToken);

            if (inventoryItem == null)
            {
                throw new KeyNotFoundException($"Inventory item with ID {request.InventoryItemId} not found");
            }

            var oldStatus = inventoryItem.Status;
            inventoryItem.Status = request.Status;

            // Add condition notes if provided
            if (!string.IsNullOrWhiteSpace(request.ConditionNotes))
            {
                var updatedNotes = string.IsNullOrWhiteSpace(inventoryItem.ConditionNotes) 
                    ? request.ConditionNotes 
                    : $"{inventoryItem.ConditionNotes}\n[{DateTime.UtcNow:yyyy-MM-dd HH:mm}] {request.ConditionNotes}";
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
                    // Item returned from rental
                    break;
                case Contracts.Enums.InventoryStatus.Damaged:
                    inventoryItem.IsRetired = true;
                    inventoryItem.RetirementDate = DateTime.UtcNow;
                    inventoryItem.RetirementReason = "Damaged";
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
            logger.LogError(ex, "Error occurred while executing UpdateInventoryStatusCommand for InventoryItemId: {InventoryItemId}", 
                request.InventoryItemId);
            throw;
        }
    }
}
