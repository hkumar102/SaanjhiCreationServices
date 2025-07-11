using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Products.Queries.GetProductsByIds;

public class GetProductsByIdsQueryHandler(
    ProductDbContext db, 
    IMapper mapper, 
    ILogger<GetProductsByIdsQueryHandler> logger)
    : IRequestHandler<GetProductsByIdsQuery, List<ProductDto>>
{
    public async Task<List<ProductDto>> Handle(GetProductsByIdsQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetProductsByIdsQuery execution for {ProductCount} product IDs", request.ProductIds.Count);

        try
        {
            if (!request.ProductIds.Any())
            {
                logger.LogDebug("No product IDs provided, returning empty list");
                return new List<ProductDto>();
            }

            logger.LogDebug("Fetching products from database for IDs: {ProductIds}", request.ProductIds);
            var products = await db.Products
                .Include(p => p.Media)
                .Where(p => request.ProductIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

            logger.LogDebug("Found {ProductCount} products in database", products.Count);

            var productDtos = mapper.Map<List<ProductDto>>(products);

            // Fetch categories for all products
            var categoryIds = products.Select(p => p.CategoryId).Distinct().ToList();
            logger.LogDebug("Fetching {CategoryCount} categories from local database", categoryIds.Count);
            
            var categories = await db.Categories
                .Where(c => categoryIds.Contains(c.Id))
                .Select(c => new { c.Id, c.Name })
                .ToListAsync(cancellationToken);
                
            logger.LogDebug("Received {CategoryCount} categories from local database", categories.Count);

            // Set category names
            logger.LogDebug("Mapping category names to products");
            foreach (var productDto in productDtos)
            {
                var product = products.FirstOrDefault(p => p.Id == productDto.Id);
                if (product != null)
                {
                    var categoryName = categories.FirstOrDefault(c => c.Id == product.CategoryId)?.Name;
                    productDto.CategoryName = categoryName ?? string.Empty;
                }
            }

            logger.LogDebug("GetProductsByIdsQuery completed successfully. Returning {ProductCount} products", productDtos.Count);
            return productDtos;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing GetProductsByIdsQuery for {ProductCount} product IDs", request.ProductIds.Count);
            throw;
        }
    }
}
