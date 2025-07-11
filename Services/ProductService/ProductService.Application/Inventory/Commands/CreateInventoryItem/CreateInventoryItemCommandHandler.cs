using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Inventory.Commands.CreateInventoryItem;

public class CreateInventoryItemCommandHandler(
    ProductDbContext db, 
    IMapper mapper,
    ILogger<CreateInventoryItemCommandHandler> logger)
    : IRequestHandler<CreateInventoryItemCommand, InventoryItemDto>
{
    public async Task<InventoryItemDto> Handle(CreateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting CreateInventoryItemCommand execution for ProductId: {ProductId}", request.ProductId);

        try
        {
            // Verify product exists and get available sizes/colors
            var product = await db.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {request.ProductId} not found");
            }

            // Validate size and color are available for this product
            if (!product.AvailableSizes.Contains(request.Size))
            {
                throw new ArgumentException($"Size '{request.Size}' is not available for this product");
            }

            if (!product.AvailableColors.Contains(request.Color))
            {
                throw new ArgumentException($"Color '{request.Color}' is not available for this product");
            }

            // Create inventory item
            var inventoryItem = new InventoryItem
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                Size = request.Size,
                Color = request.Color,
                Condition = request.Condition,
                Status = Contracts.Enums.InventoryStatus.Available,
                AcquisitionCost = request.AcquisitionCost,
                AcquisitionDate = request.AcquisitionDate ?? DateTime.UtcNow,
                ConditionNotes = request.ConditionNotes,
                IsRetired = false,
                TimesRented = 0,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            db.InventoryItems.Add(inventoryItem);
            await db.SaveChangesAsync(cancellationToken);

            logger.LogDebug("Successfully created inventory item with ID: {InventoryItemId}", 
                inventoryItem.Id);

            var inventoryItemDto = mapper.Map<InventoryItemDto>(inventoryItem);
            return inventoryItemDto;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing CreateInventoryItemCommand for ProductId: {ProductId}", request.ProductId);
            throw;
        }
    }
}
