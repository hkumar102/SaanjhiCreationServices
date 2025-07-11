using MediaService.Application.Commands;
using MediaService.Contracts.DTOs;
using MediaService.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MediaService.Application.Handlers;

public class BatchUploadProductMediaCommandHandler : IRequestHandler<BatchUploadProductMediaCommand, List<ProductMediaUploadResult>>
{
    private readonly IImageProcessingService _imageProcessingService;
    private readonly ILogger<BatchUploadProductMediaCommandHandler> _logger;

    public BatchUploadProductMediaCommandHandler(
        IImageProcessingService imageProcessingService,
        ILogger<BatchUploadProductMediaCommandHandler> logger)
    {
        _imageProcessingService = imageProcessingService;
        _logger = logger;
    }

    public async Task<List<ProductMediaUploadResult>> Handle(BatchUploadProductMediaCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting batch product media upload for {FileCount} files", request.Files.Count);

        var results = new List<ProductMediaUploadResult>();
        var successCount = 0;
        var failureCount = 0;

        foreach (var file in request.Files)
        {
            try
            {
                // Create individual upload command
                var uploadCommand = new UploadProductMediaCommand
                {
                    File = file,
                    Color = request.Color,
                    AltText = $"Product image - {file.FileName}",
                    IsPrimary = results.Count == 0, // First image is primary
                    ProcessingOptions = request.ProcessingOptions
                };

                // Use the individual handler logic
                var result = await ProcessSingleFile(uploadCommand, cancellationToken);
                results.Add(result);

                if (result.ProcessingStatus == "completed")
                {
                    successCount++;
                }
                else
                {
                    failureCount++;
                }

                _logger.LogDebug("Processed file {FileName} with status: {Status}", 
                    file.FileName, result.ProcessingStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file {FileName} in batch upload", file.FileName);
                
                // Add error result
                results.Add(new ProductMediaUploadResult
                {
                    MediaId = Guid.NewGuid(),
                    OriginalFileName = file.FileName,
                    ContentType = file.ContentType,
                    FileSize = file.Length,
                    ProcessingStatus = "failed",
                    UploadedAt = DateTime.UtcNow
                });
                
                failureCount++;
            }
        }

        _logger.LogInformation("Batch upload completed: {SuccessCount} successful, {FailureCount} failed", 
            successCount, failureCount);

        return results;
    }

    private async Task<ProductMediaUploadResult> ProcessSingleFile(UploadProductMediaCommand request, CancellationToken cancellationToken)
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
        return new ProductMediaUploadResult
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
    }

    private static bool IsValidImageFile(Microsoft.AspNetCore.Http.IFormFile file)
    {
        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
        return allowedTypes.Contains(file.ContentType.ToLower());
    }
}
