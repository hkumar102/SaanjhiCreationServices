using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Inventory.Queries.GetInventoryByProductId;

public class GetInventoryByProductIdQueryHandler(
    ProductDbContext db, 
    IMapper mapper,
    ILogger<GetInventoryByProductIdQueryHandler> logger)
    : IRequestHandler<GetInventoryByProductIdQuery, List<InventoryItemDto>>
{
    public async Task<List<InventoryItemDto>> Handle(GetInventoryByProductIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetInventoryByProductIdQuery execution for ProductId: {ProductId}", request.ProductId);

        try
        {
            // Verify product exists
            var productExists = await db.Products.AnyAsync(p => p.Id == request.ProductId, cancellationToken);
            if (!productExists)
            {
                throw new KeyNotFoundException($"Product with ID {request.ProductId} not found");
            }

            // Build query with filters
            var query = db.InventoryItems.Include(i => i.Product).ThenInclude(p => p.Media).Where(i => i.ProductId == request.ProductId);

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

            if (request.Condition.HasValue)
            {
                query = query.Where(i => i.Condition == request.Condition.Value);
            }

            if (!request.IncludeRetired)
            {
                query = query.Where(i => !i.IsRetired);
            }

            logger.LogDebug("Executing inventory query with filters: Size={Size}, Color={Color}, Status={Status}, Condition={Condition}, IncludeRetired={IncludeRetired}",
                request.Size, request.Color, request.Status, request.Condition, request.IncludeRetired);

            var inventoryItems = await query
                .OrderBy(i => i.Size)
                .ThenBy(i => i.Color)
                .ThenBy(i => i.CreatedAt)
                .ProjectTo<InventoryItemDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            logger.LogDebug("GetInventoryByProductIdQuery completed successfully. Found {Count} inventory items", inventoryItems.Count);

            return inventoryItems;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing GetInventoryByProductIdQuery for ProductId: {ProductId}", request.ProductId);
            throw;
        }
    }
}
