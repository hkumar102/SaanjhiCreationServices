using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler(
    ProductDbContext db, 
    IMapper mapper,
    ILogger<CreateProductCommandHandler> logger)
    : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting CreateProductCommand execution for product: {ProductName}", request.Name);

        try
        {
            var product = mapper.Map<Product>(request);
            product.Id = Guid.NewGuid();

            logger.LogDebug("Created product entity with ID: {ProductId}", product.Id);

            db.Products.Add(product);
            await db.SaveChangesAsync(cancellationToken);
            
            logger.LogDebug("Successfully created product with ID: {ProductId}", product.Id);
            
            return product.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating product: {ProductName}", request.Name);
            throw;
        }
    }
}
