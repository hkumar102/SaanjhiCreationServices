using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Infrastructure.Persistence;
using ProductService.Domain.Entities;

namespace ProductService.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler(
    ProductDbContext db, 
    IMapper mapper,
    ILogger<UpdateProductCommandHandler> logger) : IRequestHandler<UpdateProductCommand>
{
    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting UpdateProductCommand execution for ProductId: {ProductId}", request.Id);

        try
        {
            // Check for duplicate product name and category (excluding current product)
            if (await db.Products.AnyAsync(p => p.Name == request.Name && p.CategoryId == request.CategoryId && p.Id != request.Id, cancellationToken))
            {
                throw new Shared.ErrorHandling.BusinessRuleException($"A product with the name '{request.Name}' and already exists in this category.");
            }

            logger.LogDebug("Fetching product with ID: {ProductId} from database", request.Id);
            var product = await db.Products
                .Include(p => p.Media)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Product with ID {request.Id} not found");

            logger.LogDebug("Found product: {ProductName}, removing existing media", product.Name);

            // Remove existing media
            db.ProductMedia.RemoveRange(product.Media);
            await db.SaveChangesAsync(cancellationToken);
            
            logger.LogDebug("Removed {MediaCount} existing media items", product.Media.Count);

            // Update product properties
            mapper.Map(request, product);
            product.Media = mapper.Map<List<ProductMedia>>(request.Media);
            
            logger.LogDebug("Updated product properties and added {NewMediaCount} new media items", request.Media?.Count ?? 0);

            await db.SaveChangesAsync(cancellationToken);
            
            logger.LogDebug("UpdateProductCommand completed successfully for ProductId: {ProductId}", request.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing UpdateProductCommand for ProductId: {ProductId}", request.Id);
            throw;
        }
    }
}
