using MediaService.Application.Commands;
using MediaService.Application.Services;
using MediaService.Contracts.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;
using Imagekit.Sdk;
using Microsoft.Extensions.Configuration;

namespace MediaService.Application.Handlers;

/// <summary>
/// Enhanced handler that uses ImageKit's built-in transformations
/// instead of processing images locally
/// </summary>
public class ImageKitUploadProductMediaCommandHandler : IRequestHandler<UploadProductMediaCommand, ProductMediaUploadResult>
{
    private readonly ILogger<ImageKitUploadProductMediaCommandHandler> _logger;
    private readonly ImagekitClient _imagekitClient;
    private readonly ImageKitUrlService _urlService;
    private readonly string _urlEndpoint;

    public ImageKitUploadProductMediaCommandHandler(
        ILogger<ImageKitUploadProductMediaCommandHandler> logger,
        IConfiguration configuration,
        ImageKitUrlService urlService)
    {
        _logger = logger;
        _urlService = urlService;
        
        var publicKey = configuration["ImageKit:PublicKey"];
        var privateKey = configuration["ImageKit:PrivateKey"];
        _urlEndpoint = configuration["ImageKit:UrlEndpoint"] ?? "";

        _imagekitClient = new ImagekitClient(publicKey, privateKey, _urlEndpoint);
    }

    public async Task<ProductMediaUploadResult> Handle(UploadProductMediaCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting ImageKit product media upload for file: {FileName}", request.File.FileName);

        try
        {
            // Validate file
            if (request.File == null || request.File.Length == 0)
            {
                throw new ArgumentException("File is required and cannot be empty");
            }

            if (!IsValidImageFile(request.File))
            {
                throw new ArgumentException("File must be a valid image (jpg, jpeg, png, webp)");
            }

            const long maxFileSize = 10 * 1024 * 1024; // 10MB
            if (request.File.Length > maxFileSize)
            {
                throw new ArgumentException($"File size cannot exceed {maxFileSize / (1024 * 1024)}MB");
            }

            // Upload to ImageKit
            var uploadResult = await UploadToImageKit(request);

            // Generate variants using ImageKit URL transformations
            var variants = GenerateImageKitVariants(uploadResult.url);

            // Extract basic metadata
            var metadata = new ImageMetadata
            {
                OriginalWidth = uploadResult.width,
                OriginalHeight = uploadResult.height,
                ColorProfile = "sRGB",
                HasTransparency = request.File.ContentType == "image/png",
                DominantColor = "#000000", // Could enhance with ImageKit's color extraction
                DetectedColors = new string[0]
            };

            var result = new ProductMediaUploadResult
            {
                MediaId = Guid.NewGuid(),
                OriginalFileName = request.File.FileName,
                ContentType = request.File.ContentType,
                FileSize = request.File.Length,
                Url = variants["medium"].Url, // Use medium as main URL
                ThumbnailUrl = variants["thumbnail"].Url,
                Variants = variants,
                Metadata = metadata,
                UploadedAt = DateTime.UtcNow,
                ProcessingStatus = "completed",
                ProcessingTime = TimeSpan.FromMilliseconds(100) // ImageKit is fast!
            };

            _logger.LogDebug("Successfully processed ImageKit upload for file: {FileName}, MediaId: {MediaId}", 
                request.File.FileName, result.MediaId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing ImageKit upload for file: {FileName}. Error: {ErrorMessage}", 
                request.File?.FileName ?? "unknown", ex.Message);
            
            return new ProductMediaUploadResult
            {
                MediaId = Guid.NewGuid(),
                OriginalFileName = request.File?.FileName ?? "unknown",
                ContentType = request.File?.ContentType ?? "unknown",
                FileSize = request.File?.Length ?? 0,
                ProcessingStatus = "failed",
                UploadedAt = DateTime.UtcNow,
                ProcessingTime = TimeSpan.Zero
            };
        }
    }

    private async Task<Result> UploadToImageKit(UploadProductMediaCommand request)
    {
        try
        {
            // Read file content into byte array
            byte[] fileBytes;
            using (var stream = request.File.OpenReadStream())
            {
                fileBytes = new byte[stream.Length];
                var totalBytesRead = 0;
                
                while (totalBytesRead < stream.Length)
                {
                    var bytesRead = await stream.ReadAsync(fileBytes, totalBytesRead, (int)(stream.Length - totalBytesRead));
                    if (bytesRead == 0)
                        break;
                    totalBytesRead += bytesRead;
                }
            }

            // Validate we have data
            if (fileBytes.Length == 0)
            {
                throw new InvalidOperationException("File content is empty");
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{request.File.FileName}";

            var uploadRequest = new FileCreateRequest
            {
                file = fileBytes,
                fileName = uniqueFileName,
                folder = "/products/",
                useUniqueFileName = false,
                isPrivateFile = false,
                tags = new List<string> { "product", request.Color ?? "unspecified", request.IsPrimary ? "primary" : "secondary" },
                customMetadata = new System.Collections.Hashtable
                {
                    { "altText", request.AltText ?? "" },
                    { "color", request.Color ?? "" },
                    { "isPrimary", request.IsPrimary.ToString() }
                }
            };

            _logger.LogDebug("Uploading file to ImageKit: {FileName}, Size: {Size} bytes", 
                uniqueFileName, fileBytes.Length);

            var result = await _imagekitClient.UploadAsync(uploadRequest);
            
            if (result == null || string.IsNullOrEmpty(result.url))
            {
                throw new InvalidOperationException("ImageKit upload failed - no URL returned");
            }

            _logger.LogDebug("ImageKit upload successful: {Url}", result.url);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file to ImageKit: {FileName}", request.File.FileName);
            throw;
        }
    }

    private Dictionary<string, ProcessedImageVariant> GenerateImageKitVariants(string originalUrl)
    {
        // Using ImageKit's real-time transformations instead of pre-processing
        var variants = new Dictionary<string, ProcessedImageVariant>
        {
            ["thumbnail"] = new ProcessedImageVariant
            {
                Url = _urlService.GetThumbnailUrl(originalUrl),
                Width = 150,
                Height = 150,
                FileSize = 0, // ImageKit calculates on-demand
                Format = "auto", // ImageKit auto-selects best format
                Usage = "list_view"
            },
            ["small"] = new ProcessedImageVariant
            {
                Url = _urlService.GetSmallUrl(originalUrl),
                Width = 400,
                Height = 400,
                FileSize = 0,
                Format = "auto",
                Usage = "grid_view"
            },
            ["medium"] = new ProcessedImageVariant
            {
                Url = _urlService.GetMediumUrl(originalUrl),
                Width = 800,
                Height = 800,
                FileSize = 0,
                Format = "auto",
                Usage = "detail_view"
            },
            ["large"] = new ProcessedImageVariant
            {
                Url = _urlService.GetLargeUrl(originalUrl),
                Width = 1200,
                Height = 1200,
                FileSize = 0,
                Format = "auto",
                Usage = "zoom_view"
            },
            ["original"] = new ProcessedImageVariant
            {
                Url = originalUrl,
                Width = 0, // Will be set from upload result
                Height = 0,
                FileSize = 0,
                Format = "original",
                Usage = "print_quality"
            }
        };

        return variants;
    }

    private static bool IsValidImageFile(Microsoft.AspNetCore.Http.IFormFile file)
    {
        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
        return allowedTypes.Contains(file.ContentType.ToLower());
    }
}
