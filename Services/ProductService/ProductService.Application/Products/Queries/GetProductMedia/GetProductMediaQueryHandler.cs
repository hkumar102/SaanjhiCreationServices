using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Products.Queries.GetProductMedia;

public class GetProductMediaQueryHandler(
    ProductDbContext db, 
    IMapper mapper,
    ILogger<GetProductMediaQueryHandler> logger)
    : IRequestHandler<GetProductMediaQuery, List<ProductMediaDto>>
{
    public async Task<List<ProductMediaDto>> Handle(GetProductMediaQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetProductMediaQuery execution for ProductId: {ProductId}", request.ProductId);

        try
        {
            // Verify product exists
            var productExists = await db.Products.Include(p => p.Category).AnyAsync(p => p.Id == request.ProductId, cancellationToken);
            if (!productExists)
            {
                throw new KeyNotFoundException($"Product with ID {request.ProductId} not found");
            }

            // Build query with filters
            var query = db.ProductMedia.Where(m => m.ProductId == request.ProductId);

            // Apply color filter if specified
            if (!string.IsNullOrWhiteSpace(request.Color))
            {
                query = query.Where(m => m.Color == request.Color || m.IsGeneric);
            }

            logger.LogDebug("Executing media query with filters: Color={Color}", request.Color);

            var mediaItems = await query
                .OrderBy(m => m.SortOrder)
                .ThenBy(m => m.CreatedAt)
                .ProjectTo<ProductMediaDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            logger.LogDebug("GetProductMediaQuery completed successfully. Found {Count} media items", mediaItems.Count);

            return mediaItems;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing GetProductMediaQuery for ProductId: {ProductId}", request.ProductId);
            throw;
        }
    }
}
