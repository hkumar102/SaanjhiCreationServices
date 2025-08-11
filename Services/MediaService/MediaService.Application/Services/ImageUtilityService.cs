using MediaService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;

namespace MediaService.Application.Services;

public class ImageUtilityService
(
    ILogger<ImageUtilityService> _logger
) : IImageUtlityService
{
    public async Task<(byte[] data, string contentType, int width, int height)> CompressImage(IFormFile file)
    {
        using var inputStream = file.OpenReadStream();
        using var image = await Image.LoadAsync(inputStream);
        
        // Configuration for optimization
        const int maxWidth = 1024;  // Max width for uploaded images
        const int maxHeight = 1024; // Max height for uploaded images
        const int jpegQuality = 85;  // Good balance between quality and file size
        
        var originalWidth = image.Width;
        var originalHeight = image.Height;
        
        // Resize if image is too large
        if (image.Width > maxWidth || image.Height > maxHeight)
        {
            var resizeOptions = new ResizeOptions
            {
                Size = new Size(maxWidth, maxHeight),
                Mode = ResizeMode.Max // Preserve aspect ratio, fit within box
            };
            _logger.LogDebug("Resizing image from {OriginalWidth}x{OriginalHeight} to fit within {MaxWidth}x{MaxHeight}", 
                originalWidth, originalHeight, maxWidth, maxHeight);
            image.Mutate(x => x.Resize(resizeOptions));
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
}