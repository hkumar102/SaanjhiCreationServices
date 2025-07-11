using MediaService.Application.Commands;
using MediaService.Contracts.DTOs;
using MediaService.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MediaService.Application.Handlers;

public class UploadProductMediaCommandHandler : IRequestHandler<UploadProductMediaCommand, ProductMediaUploadResult>
{
    private readonly IImageProcessingService _imageProcessingService;
    private readonly ILogger<UploadProductMediaCommandHandler> _logger;

    public UploadProductMediaCommandHandler(
        IImageProcessingService imageProcessingService,
        ILogger<UploadProductMediaCommandHandler> logger)
    {
        _imageProcessingService = imageProcessingService;
        _logger = logger;
    }

    public async Task<ProductMediaUploadResult> Handle(UploadProductMediaCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting product media upload for file: {FileName}", request.File.FileName);

        try
        {
            // Validate file
            if (request.File == null || request.File.Length == 0)
            {
                throw new ArgumentException("File is required and cannot be empty");
            }

            // Validate file type
            if (!IsValidImageFile(request.File))
            {
                throw new ArgumentException("File must be a valid image (jpg, jpeg, png, webp)");
            }

            // Validate file size (e.g., 10MB limit)
            const long maxFileSize = 10 * 1024 * 1024; // 10MB
            if (request.File.Length > maxFileSize)
            {
                throw new ArgumentException($"File size cannot exceed {maxFileSize / (1024 * 1024)}MB");
            }

            // Process the image
            using var stream = request.File.OpenReadStream();
            var processedResult = await _imageProcessingService.ProcessImageAsync(
                stream, 
                request.File.FileName, 
                request.ProcessingOptions);

            // Create the result
            var result = new ProductMediaUploadResult
            {
                MediaId = Guid.NewGuid(),
                OriginalFileName = request.File.FileName,
                ContentType = request.File.ContentType,
                FileSize = request.File.Length,
                Url = processedResult.MainUrl,
                ThumbnailUrl = processedResult.ThumbnailUrl,
                Variants = processedResult.Variants,
                Metadata = processedResult.Metadata,
                UploadedAt = DateTime.UtcNow,
                ProcessingStatus = "completed",
                ProcessingTime = processedResult.ProcessingTime
            };

            _logger.LogDebug("Successfully processed product media upload for file: {FileName}, MediaId: {MediaId}", 
                request.File.FileName, result.MediaId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing product media upload for file: {FileName}", request.File.FileName);
            
            // Return error result
            return new ProductMediaUploadResult
            {
                MediaId = Guid.NewGuid(),
                OriginalFileName = request.File.FileName,
                ContentType = request.File.ContentType,
                FileSize = request.File.Length,
                ProcessingStatus = "failed",
                UploadedAt = DateTime.UtcNow
            };
        }
    }

    private static bool IsValidImageFile(Microsoft.AspNetCore.Http.IFormFile file)
    {
        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
        return allowedTypes.Contains(file.ContentType.ToLower());
    }
}
