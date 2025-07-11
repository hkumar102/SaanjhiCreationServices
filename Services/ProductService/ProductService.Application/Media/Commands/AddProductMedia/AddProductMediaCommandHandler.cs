using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Media.Commands.AddProductMedia;

public class AddProductMediaCommandHandler(
    ProductDbContext db, 
    IMapper mapper,
    ILogger<AddProductMediaCommandHandler> logger)
    : IRequestHandler<AddProductMediaCommand, ProductMediaDto>
{
    public async Task<ProductMediaDto> Handle(AddProductMediaCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting AddProductMediaCommand execution for ProductId: {ProductId}", request.ProductId);

        try
        {
            // Verify product exists and get available colors if not generic
            var product = await db.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {request.ProductId} not found");
            }

            // Validate color if specified and not generic
            if (!request.IsGeneric && !string.IsNullOrWhiteSpace(request.Color) && 
                !product.AvailableColors.Contains(request.Color))
            {
                throw new ArgumentException($"Color '{request.Color}' is not available for this product");
            }

            // Create media item
            var productMedia = new ProductMedia
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                Url = request.Url,
                MediaType = request.MediaType,
                Color = request.IsGeneric ? null : request.Color,
                SortOrder = request.SortOrder,
                IsGeneric = request.IsGeneric,
                AltText = request.AltText,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            db.ProductMedia.Add(productMedia);
            await db.SaveChangesAsync(cancellationToken);

            logger.LogDebug("Successfully created product media with ID: {MediaId} for ProductId: {ProductId}", 
                productMedia.Id, request.ProductId);

            var productMediaDto = mapper.Map<ProductMediaDto>(productMedia);
            return productMediaDto;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing AddProductMediaCommand for ProductId: {ProductId}", request.ProductId);
            throw;
        }
    }
}
