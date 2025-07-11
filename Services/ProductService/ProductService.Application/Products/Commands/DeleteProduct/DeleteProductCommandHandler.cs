using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler(
    ProductDbContext db,
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

            logger.LogDebug("Found product: {ProductName}, proceeding with deletion", product.Name);

            db.Products.Remove(product);
            await db.SaveChangesAsync(cancellationToken);
            
            logger.LogDebug("DeleteProductCommand completed successfully for ProductId: {ProductId}", request.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing DeleteProductCommand for ProductId: {ProductId}", request.Id);
            throw;
        }
    }
}
