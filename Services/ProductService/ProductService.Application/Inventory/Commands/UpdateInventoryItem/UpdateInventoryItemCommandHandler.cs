using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.Persistence;
using Shared.ErrorHandling;

namespace ProductService.Application.Inventory.Commands.UpdateInventoryItem;

public class UpdateInventoryItemCommandHandler(
    ProductDbContext db,
    IMapper mapper,
    ILogger<UpdateInventoryItemCommandHandler> logger)
    : IRequestHandler<UpdateInventoryItemCommand, InventoryItemDto>
{
    public async Task<InventoryItemDto> Handle(UpdateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting UpdateInventoryItemCommand for InventoryItemId: {InventoryItemId}", request.Id);
        try
        {
            var item = await db.InventoryItems.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
            if (item == null)
                throw new KeyNotFoundException($"InventoryItem with ID {request.Id} not found");

            // Update fields if provided
            if (request.Size != null) item.Size = request.Size;
            if (request.Color != null) item.Color = request.Color;
            if (request.Condition.HasValue) item.Condition = request.Condition.Value;
            if (request.AcquisitionCost.HasValue) item.AcquisitionCost = request.AcquisitionCost.Value;
            if (request.AcquisitionDate.HasValue) item.AcquisitionDate = request.AcquisitionDate.Value;
            if (request.ConditionNotes != null) item.ConditionNotes = request.ConditionNotes;
            if (request.WarehouseLocation != null) item.WarehouseLocation = request.WarehouseLocation;
            if (request.IsRetired.HasValue)
            {
                // Handle retirement logic
                // if status is rented, then inventory item cannot be retired
                if (item.Status == Contracts.Enums.InventoryStatus.Rented && request.IsRetired.Value)
                {
                    logger.LogError("Cannot retire an inventory item that is currently rented. InventoryItemId: {InventoryItemId}", item.Id);
                    throw new BusinessRuleException("Cannot retire an inventory item that is currently rented.");
                }

                item.IsRetired = request.IsRetired.Value;
                item.Status = Contracts.Enums.InventoryStatus.Retired;
                if (request.IsRetired.Value)
                {
                    item.RetirementDate = DateTime.UtcNow;
                    item.RetirementReason = request.RetirementReason ?? "Retired by user";
                }
                else
                {
                    item.RetirementDate = null;
                    item.RetirementReason = null;
                }
            }

            await db.SaveChangesAsync(cancellationToken);
            logger.LogDebug("Successfully updated inventory item with ID: {InventoryItemId}", item.Id);
            return mapper.Map<InventoryItemDto>(item);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating InventoryItem with ID: {InventoryItemId}", request.Id);
            throw;
        }
    }
}
