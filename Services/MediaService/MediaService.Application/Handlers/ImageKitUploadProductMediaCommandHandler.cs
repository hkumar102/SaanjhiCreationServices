using MediaService.Application.Commands;
using MediaService.Application.Services;
using MediaService.Contracts.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;
using Imagekit.Sdk;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;

namespace MediaService.Application.Handlers;

/// <summary>
/// Enhanced handler that uses ImageKit's built-in transformations
/// and optimizes images before upload to save on ImageKit storage (important for free tier)
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

            const long maxFileSize = 50 * 1024 * 1024; // 50MB - Allow larger input since we'll compress
            if (request.File.Length > maxFileSize)
            {
                throw new ArgumentException($"Input file size cannot exceed {maxFileSize / (1024 * 1024)}MB");
            }

            // Compress and optimize image before upload to save ImageKit storage
            var optimizedImageData = await CompressImageForUpload(request.File);

            // Upload to ImageKit
            var uploadResult = await UploadToImageKit(request, optimizedImageData);

            // Generate variants using ImageKit URL transformations
            var variants = GenerateImageKitVariants(uploadResult.url);

            // Extract basic metadata
            var metadata = new ImageMetadata
            {
                OriginalWidth = optimizedImageData.width,
                OriginalHeight = optimizedImageData.height,
                ColorProfile = "sRGB",
                HasTransparency = optimizedImageData.contentType == "image/png",
                DominantColor = "#000000", // Could enhance with ImageKit's color extraction
                DetectedColors = new string[0]
            };

            var result = new ProductMediaUploadResult
            {
                MediaId = Guid.NewGuid(),
                OriginalFileName = request.File.FileName,
                ContentType = optimizedImageData.contentType,
                FileSize = optimizedImageData.data.Length, // Use optimized file size
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

    /// <summary>
    /// Compress and optimize image before uploading to save ImageKit storage
    /// </summary>
    private async Task<(byte[] data, string contentType, int width, int height)> CompressImageForUpload(Microsoft.AspNetCore.Http.IFormFile file)
    {
        using var inputStream = file.OpenReadStream();
        using var image = await Image.LoadAsync(inputStream);
        
        // Configuration for optimization
        const int maxWidth = 2048;  // Max width for uploaded images
        const int maxHeight = 2048; // Max height for uploaded images
        const int jpegQuality = 85;  // Good balance between quality and file size
        
        var originalWidth = image.Width;
        var originalHeight = image.Height;
        
        // Resize if image is too large
        if (image.Width > maxWidth || image.Height > maxHeight)
        {
            var aspectRatio = (double)image.Width / image.Height;
            
            int newWidth, newHeight;
            if (aspectRatio > 1) // Landscape
            {
                newWidth = Math.Min(maxWidth, image.Width);
                newHeight = (int)(newWidth / aspectRatio);
            }
            else // Portrait or square
            {
                newHeight = Math.Min(maxHeight, image.Height);
                newWidth = (int)(newHeight * aspectRatio);
            }
            
            _logger.LogDebug("Resizing image from {OriginalWidth}x{OriginalHeight} to {NewWidth}x{NewHeight}", 
                originalWidth, originalHeight, newWidth, newHeight);
            
            image.Mutate(x => x.Resize(newWidth, newHeight));
        }
        
        // Compress based on file type
        using var outputStream = new MemoryStream();
        string contentType;
        
        if (file.ContentType.ToLower().Contains("png"))
        {
            // For PNG, use PNG encoder with compression
            var pngEncoder = new PngEncoder
            {
                CompressionLevel = PngCompressionLevel.BestCompression
            };
            await image.SaveAsync(outputStream, pngEncoder);
            contentType = "image/png";
        }
        else if (file.ContentType.ToLower().Contains("webp"))
        {
            // For WebP, use high quality compression
            var webpEncoder = new WebpEncoder
            {
                Quality = jpegQuality
            };
            await image.SaveAsync(outputStream, webpEncoder);
            contentType = "image/webp";
        }
        else
        {
            // Default to JPEG for all other formats (including original JPEG)
            var jpegEncoder = new JpegEncoder
            {
                Quality = jpegQuality
            };
            await image.SaveAsync(outputStream, jpegEncoder);
            contentType = "image/jpeg";
        }
        
        var compressedData = outputStream.ToArray();
        var compressionRatio = (double)compressedData.Length / file.Length * 100;
        
        _logger.LogDebug("Image compression complete. Original: {OriginalSize} bytes, Compressed: {CompressedSize} bytes ({CompressionRatio:F1}%)", 
            file.Length, compressedData.Length, compressionRatio);
        
        return (compressedData, contentType, image.Width, image.Height);
    }

    private async Task<Result> UploadToImageKit(UploadProductMediaCommand request, (byte[] data, string contentType, int width, int height) optimizedImage)
    {
        try
        {
            var uniqueFileName = $"{Guid.NewGuid()}_{request.File.FileName}";

            var uploadRequest = new FileCreateRequest
            {
                file = optimizedImage.data,
                fileName = uniqueFileName,
                folder = "/products/",
                useUniqueFileName = false,
                isPrivateFile = false,
                tags = new List<string> { "product", request.Color ?? "unspecified", request.IsPrimary ? "primary" : "secondary" },
                customMetadata = new System.Collections.Hashtable
                {
                    { "altText", request.AltText ?? "" },
                    { "color", request.Color ?? "" },
                    { "isPrimary", request.IsPrimary }
                }
            };

            _logger.LogDebug("Uploading optimized file to ImageKit: {FileName}, Original Size: {OriginalSize} bytes, Optimized Size: {OptimizedSize} bytes", 
                uniqueFileName, request.File.Length, optimizedImage.data.Length);

            var result = await _imagekitClient.UploadAsync(uploadRequest);
            
            if (result == null || string.IsNullOrEmpty(result.url))
            {
                _logger.LogError("ImageKit upload failed - no URL returned for file: {FileName}", result?.Raw);
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
