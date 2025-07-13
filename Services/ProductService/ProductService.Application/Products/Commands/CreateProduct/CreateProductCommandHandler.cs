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
            product.SKU = GenerateSku(request.Name, request.CategoryId);

            logger.LogDebug("Created product entity with ID: {ProductId} and SKU: {Sku}", product.Id, product.SKU);

            db.Products.Add(product);
            await db.SaveChangesAsync(cancellationToken);
            
            logger.LogDebug("Successfully created product with ID: {ProductId} and SKU: {Sku}", product.Id, product.SKU);
            
            return product.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating product: {ProductName}", request.Name);
            throw;
        }
    }

    private static string GenerateSku(string? name, Guid categoryId)
    {
        var categoryCode = categoryId.ToString().Substring(0, 4).ToUpper();
        var slug = (name ?? "product").ToLower().Replace(" ", "-");
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        return $"SKU-{slug}-{categoryCode}-{timestamp}";
    }
}
