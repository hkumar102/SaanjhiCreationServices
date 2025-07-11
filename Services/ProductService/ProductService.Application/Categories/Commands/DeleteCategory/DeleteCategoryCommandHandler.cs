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
            // Check if category has any products before deleting
            var hasProducts = await db.Products.AnyAsync(p => p.CategoryId == request.Id, cancellationToken);
            if (hasProducts)
            {
                throw new InvalidOperationException($"Cannot delete category with ID {request.Id} because it has associated products");
            }

            var category = await db.Categories.FindAsync(new object[] { request.Id }, cancellationToken)
                ?? throw new KeyNotFoundException($"Category with ID {request.Id} not found");

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
