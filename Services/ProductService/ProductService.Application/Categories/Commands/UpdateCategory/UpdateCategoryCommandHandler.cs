using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler(
    ProductDbContext db,
    ILogger<UpdateCategoryCommandHandler> logger) 
    : IRequestHandler<UpdateCategoryCommand>
{
    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting UpdateCategoryCommand execution for category ID: {CategoryId}", request.Id);

        try
        {
            var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Category with ID {request.Id} not found");

            category.Name = request.Name;
            category.Description = request.Description;

            await db.SaveChangesAsync(cancellationToken);
            
            logger.LogDebug("Successfully updated category with ID: {CategoryId}", request.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating category with ID: {CategoryId}", request.Id);
            throw;
        }
    }
}
