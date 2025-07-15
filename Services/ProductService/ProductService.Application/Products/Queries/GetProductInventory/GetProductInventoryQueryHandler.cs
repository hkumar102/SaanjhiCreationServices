using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Products.Queries.GetProductInventory;

public class GetProductInventoryQueryHandler(
    ProductDbContext db, 
    IMapper mapper,
    ILogger<GetProductInventoryQueryHandler> logger)
    : IRequestHandler<GetProductInventoryQuery, List<InventoryItemDto>>
{
    public async Task<List<InventoryItemDto>> Handle(GetProductInventoryQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetProductInventoryQuery execution for ProductId: {ProductId}", request.ProductId);

        try
        {
            // Verify product exists
            var productExists = await db.Products.AnyAsync(p => p.Id == request.ProductId, cancellationToken);
            if (!productExists)
            {
                throw new KeyNotFoundException($"Product with ID {request.ProductId} not found");
            }

            // Build query with filters - using navigation property from Product
            var query = db.Products
                .Where(p => p.Id == request.ProductId)
                .SelectMany(p => p.InventoryItems);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(request.Size))
            {
                query = query.Where(i => i.Size == request.Size);
            }

            if (!string.IsNullOrWhiteSpace(request.Color))
            {
                query = query.Where(i => i.Color == request.Color);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(i => i.Status == request.Status.Value);
            }

            if (!request.IncludeRetired)
            {
                query = query.Where(i => !i.IsRetired);
            }

            logger.LogDebug("Executing inventory query with filters: Size={Size}, Color={Color}, Status={Status}, IncludeRetired={IncludeRetired}",
                request.Size, request.Color, request.Status, request.IncludeRetired);

            var inventoryItems = await query
                .OrderBy(i => i.Size)
                .ThenBy(i => i.Color)
                .ThenBy(i => i.AcquisitionDate)
                .ProjectTo<InventoryItemDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            logger.LogDebug("GetProductInventoryQuery completed successfully. Found {Count} inventory items", inventoryItems.Count);

            return inventoryItems;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing GetProductInventoryQuery for ProductId: {ProductId}", request.ProductId);
            throw;
        }
    }
}
