using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Infrastructure.Persistence;
using Shared.Application.Interfaces;
using Shared.Domain.Extensions;

namespace ProductService.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler(
    ProductDbContext db,
    ICurrentUserService currentUserService,
    ILogger<DeleteProductCommandHandler> logger) : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting DeleteProductCommand execution for ProductId: {ProductId}", request.Id);

        try
        {
            logger.LogDebug("Fetching product with ID: {ProductId} from database", request.Id);
            var product = await db.Products.FindAsync(new object[] { request.Id }, cancellationToken)
                ?? throw new KeyNotFoundException($"Product with ID {request.Id} not found");

            logger.LogDebug("Found product: {ProductName}, proceeding with soft deletion", product.Name);

            // Use soft delete instead of hard delete
            product.SoftDelete(currentUserService.UserId);
            await db.SaveChangesAsync(cancellationToken);
            
            logger.LogDebug("DeleteProductCommand completed successfully for ProductId: {ProductId} (soft deleted)", request.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing DeleteProductCommand for ProductId: {ProductId}", request.Id);
            throw;
        }
    }
}
