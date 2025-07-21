using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler(
    ProductDbContext db, 
    IMapper mapper, 
    ILogger<GetProductByIdQueryHandler> logger)
    : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetProductByIdQuery execution for ProductId: {ProductId}", request.Id);

        try
        {
            logger.LogDebug("Fetching product with ID: {ProductId} from database", request.Id);
            
            // Build query with conditional includes
            var query = db.Products.Include(p => p.Category).AsQueryable();

            if (request.IncludeMedia)
            {
                query = query.Include(p => p.Media);
            }

            if (request.IncludeInventory)
            {
                query = query.Include(p => p.InventoryItems);
            }

            var product = await query
                              .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
                          ?? throw new KeyNotFoundException($"Product with ID {request.Id} not found");
            
            logger.LogDebug("Product found: {ProductName}, CategoryId: {CategoryId}", product.Name, product.CategoryId);
            
            var productDto = mapper.Map<ProductDto>(product);
            
            // Enhance DTO with additional data
            await EnhanceProductDto(productDto, request, cancellationToken);
            
            logger.LogDebug("GetProductByIdQuery completed successfully for ProductId: {ProductId}", request.Id);

            return productDto;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing GetProductByIdQuery for ProductId: {ProductId}", request.Id);
            throw;
        }
    }

    private async Task EnhanceProductDto(ProductDto productDto, GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        // Calculate inventory counts
        CalculateInventoryCounts(productDto);

        // Organize media by color if requested
        if (request.OrganizeMediaByColor && request.IncludeMedia)
        {
            OrganizeMediaByColor(productDto);
        }
    }

    private void CalculateInventoryCounts(ProductDto productDto)
    {
        if (productDto.InventoryItems != null)
        {
            productDto.TotalInventoryCount = productDto.InventoryItems.Count;
            productDto.AvailableInventoryCount = productDto.InventoryItems.Count(i => 
                i.Status == Contracts.Enums.InventoryStatus.Available && !i.IsRetired);
        }
    }

    private void OrganizeMediaByColor(ProductDto productDto)
    {
        if (productDto.Media?.Any() == true)
        {
            // Organize media by color
            productDto.MediaByColor = productDto.Media
                .Where(m => !string.IsNullOrEmpty(m.Color) && !m.IsGeneric)
                .GroupBy(m => m.Color!)
                .ToDictionary(g => g.Key, g => g.OrderBy(m => m.SortOrder).ToList());

            // Separate generic media
            productDto.GenericMedia = productDto.Media
                .Where(m => m.IsGeneric)
                .OrderBy(m => m.SortOrder)
                .ToList();

            logger.LogDebug("Organized media: {ColorCount} colors, {GenericCount} generic items", 
                productDto.MediaByColor.Count, productDto.GenericMedia.Count);
        }
    }
}
