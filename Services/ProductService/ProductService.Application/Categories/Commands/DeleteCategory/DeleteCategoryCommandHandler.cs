using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler(
    ProductDbContext db,
    ILogger<DeleteCategoryCommandHandler> logger) 
    : IRequestHandler<DeleteCategoryCommand>
{
    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting DeleteCategoryCommand execution for category ID: {CategoryId}", request.Id);

        try
        {
            // Check if category has any active products before deleting
            var hasActiveProducts = await db.Products
                .Where(p => p.CategoryId == request.Id && !p.IsDeleted)
                .AnyAsync(cancellationToken);
                
            if (hasActiveProducts)
            {
                throw new InvalidOperationException($"Cannot delete category with ID {request.Id} because it has associated active products");
            }

            var category = await db.Categories.FindAsync(new object[] { request.Id }, cancellationToken)
                ?? throw new KeyNotFoundException($"Category with ID {request.Id} not found");

            // Use standard Remove() - DbContext will automatically convert to soft delete
            db.Categories.Remove(category);
            await db.SaveChangesAsync(cancellationToken);
            
            logger.LogDebug("Successfully deleted category with ID: {CategoryId}", request.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting category with ID: {CategoryId}", request.Id);
            throw;
        }
    }
}
