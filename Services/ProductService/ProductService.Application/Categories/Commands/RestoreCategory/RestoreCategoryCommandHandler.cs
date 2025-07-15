using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Infrastructure.Persistence;
using Shared.Domain.Extensions;
using Shared.Infrastructure.Extensions;

namespace ProductService.Application.Categories.Commands.RestoreCategory;

public class RestoreCategoryCommandHandler(
    ProductDbContext db,
    ILogger<RestoreCategoryCommandHandler> logger) 
    : IRequestHandler<RestoreCategoryCommand>
{
    public async Task Handle(RestoreCategoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting RestoreCategoryCommand execution for category ID: {CategoryId}", request.Id);

        try
        {
            // Find soft deleted category (bypass query filter)
            var category = await db.GetWithSoftDeleted<ProductService.Domain.Entities.Category>()
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Category with ID {request.Id} not found");

            if (!category.IsDeleted)
            {
                throw new InvalidOperationException($"Category with ID {request.Id} is not deleted");
            }

            // Restore the category using extension method
            category.Restore();
            await db.SaveChangesAsync(cancellationToken);
            
            logger.LogDebug("Successfully restored category with ID: {CategoryId}", request.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while restoring category with ID: {CategoryId}", request.Id);
            throw;
        }
    }
}
