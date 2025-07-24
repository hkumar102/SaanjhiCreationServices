using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
            // Check for duplicate product name and category
            if (await db.Products.AnyAsync(p => p.Name == request.Name && p.CategoryId == request.CategoryId, cancellationToken))
            {
                throw new Shared.ErrorHandling.BusinessRuleException($"A product with the name '{request.Name}' already exists in this category.");
            }

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
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var sku = $"SKU-{timestamp}";
        return sku;
    }
}
