using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.Services;
using ProductService.Contracts.DTOs;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Persistence;
using Shared.Contracts.Media;

namespace ProductService.Application.Media.Commands.UploadProductMedia;

public class UploadProductMediaCommandHandler : IRequestHandler<UploadProductMediaCommand, ProductMediaDto>
{
    private readonly ProductDbContext _db;
    private readonly IMapper _mapper;
    private readonly ILogger<UploadProductMediaCommandHandler> _logger;
    private readonly IMediaServiceClient _mediaServiceClient;

    public UploadProductMediaCommandHandler(
        ProductDbContext db,
        IMapper mapper,
        ILogger<UploadProductMediaCommandHandler> logger,
        IMediaServiceClient mediaServiceClient)
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
        _mediaServiceClient = mediaServiceClient;
    }

    public async Task<ProductMediaDto> Handle(UploadProductMediaCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting UploadProductMediaCommand execution for ProductId: {ProductId}", request.ProductId);

        try
        {
            // Verify product exists
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {request.ProductId} not found");
            }

            // Upload to MediaService
            var mediaResponse = await _mediaServiceClient.UploadProductImageAsync(
                request.File, 
                request.IsPrimary, 
                request.Color, 
                request.AltText);

            // If this is marked as primary, update existing primary images
            if (request.IsPrimary)
            {
                var existingPrimary = await _db.ProductMedia
                    .Where(pm => pm.ProductId == request.ProductId && pm.IsPrimary)
                    .ToListAsync(cancellationToken);

                foreach (var existing in existingPrimary)
                {
                    existing.IsPrimary = false;
                    existing.ModifiedAt = DateTime.UtcNow;
                }
            }

            // Create ProductMedia entity from MediaService response
            var productMedia = new ProductMedia
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                MediaId = mediaResponse.MediaId,
                Url = mediaResponse.Url,
                ThumbnailUrl = mediaResponse.ThumbnailUrl,
                IsPrimary = request.IsPrimary,
                DisplayOrder = request.DisplayOrder,
                OriginalFileName = mediaResponse.OriginalFileName,
                ContentType = mediaResponse.ContentType,
                FileSize = mediaResponse.FileSize,
                Width = mediaResponse.Metadata.OriginalWidth,
                Height = mediaResponse.Metadata.OriginalHeight,
                AltText = request.AltText,
                Color = request.Color,
                UploadedAt = mediaResponse.UploadedAt,
                ProcessingStatus = mediaResponse.ProcessingStatus,
                VariantsJson = JsonSerializer.Serialize(mediaResponse.Variants),
                MediaType = MediaType.Image,
                SortOrder = request.DisplayOrder,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            _db.ProductMedia.Add(productMedia);
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Successfully created ProductMedia for ProductId: {ProductId}, MediaId: {MediaId}", 
                request.ProductId, mediaResponse.MediaId);

            return _mapper.Map<ProductMediaDto>(productMedia);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing UploadProductMediaCommand for ProductId: {ProductId}", request.ProductId);
            throw;
        }
    }
}
