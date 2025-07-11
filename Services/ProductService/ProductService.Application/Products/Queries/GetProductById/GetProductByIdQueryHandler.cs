using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.HttpClients;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler(
    ProductDbContext db, 
    IMapper mapper, 
    CategoryApiClient categoryApiClient,
    ILogger<GetProductByIdQueryHandler> logger)
    : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetProductByIdQuery execution for ProductId: {ProductId}", request.Id);

        try
        {
            logger.LogDebug("Fetching product with ID: {ProductId} from database", request.Id);
            var product = await db.Products
                              .Include(p => p.Media)
                              .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
                          ?? throw new KeyNotFoundException($"Product with ID {request.Id} not found");
            
            logger.LogDebug("Product found: {ProductName}, CategoryId: {CategoryId}", product.Name, product.CategoryId);
            
            var productDto = mapper.Map<ProductDto>(product);
            
            // Fetch category from CategoryApiClient
            logger.LogDebug("Fetching category with ID: {CategoryId} from CategoryService", product.CategoryId);
            var category = await categoryApiClient.GetCategoryByIdAsync(product.CategoryId);
            productDto.CategoryName = category?.Name;
            
            logger.LogDebug("Successfully retrieved category: {CategoryName}", category?.Name);
            logger.LogDebug("GetProductByIdQuery completed successfully for ProductId: {ProductId}", request.Id);

            return productDto;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing GetProductByIdQuery for ProductId: {ProductId}", request.Id);
            throw;
        }
    }
}
