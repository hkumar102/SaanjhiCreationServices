using MediaService.Contracts.DTOs;
using MediaService.Contracts.Interfaces;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.PixelFormats;

namespace MediaService.Application.Services;

/// <summary>
/// Service for image processing operations
/// </summary>
public interface IImageProcessingService
{
    Task<ProcessedImageResult> ProcessImageAsync(Stream imageStream, string fileName, ProductImageProcessingOptions options);
    Task<Dictionary<string, ProcessedImageVariant>> GenerateVariantsAsync(Image image, ProductImageProcessingOptions options, string baseFileName);
    ImageMetadata ExtractMetadata(Image image);
    string DetectDominantColor(Image image);
}

public class ImageProcessingService : IImageProcessingService
{
    private readonly ILogger<ImageProcessingService> _logger;
    private readonly IFileStorageService _fileStorage;

    public ImageProcessingService(ILogger<ImageProcessingService> logger, IFileStorageService fileStorage)
    {
        _logger = logger;
        _fileStorage = fileStorage;
    }

    public async Task<ProcessedImageResult> ProcessImageAsync(Stream imageStream, string fileName, ProductImageProcessingOptions options)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            using var image = await Image.LoadAsync(imageStream);
            
            // Extract metadata first
            var metadata = ExtractMetadata(image);
            
            // Auto-orient the image
            if (options.AutoOrient)
            {
                image.Mutate(x => x.AutoOrient());
            }
            
            // Remove metadata if requested
            if (options.RemoveMetadata)
            {
                image.Metadata.ExifProfile = null;
                image.Metadata.IptcProfile = null;
                image.Metadata.XmpProfile = null;
            }
            
            // Generate all variants
            var variants = await GenerateVariantsAsync(image, options, fileName);
            
            // Get the main URLs
            var mainVariant = variants["medium"]; // Use medium as main
            var thumbnailVariant = variants["thumbnail"];
            
            stopwatch.Stop();
            
            return new ProcessedImageResult
            {
                MainUrl = mainVariant.Url,
                ThumbnailUrl = thumbnailVariant.Url,
                Variants = variants,
                Metadata = metadata,
                ProcessingTime = stopwatch.Elapsed,
                TotalFileSize = variants.Values.Sum(v => v.FileSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing image: {FileName}", fileName);
            throw new InvalidOperationException($"Failed to process image: {ex.Message}", ex);
        }
    }

    public async Task<Dictionary<string, ProcessedImageVariant>> GenerateVariantsAsync(
        Image image, 
        ProductImageProcessingOptions options, 
        string baseFileName)
    {
        var variants = new Dictionary<string, ProcessedImageVariant>();
        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(baseFileName);
        
        foreach (var size in options.GenerateSizes)
        {
            try
            {
                // Create a copy of the image for resizing
                var resizedImage = image.CloneAs<Rgba32>();
                
                // Resize the image
                var resizeOptions = new ResizeOptions
                {
                    Size = new Size(size.Width, size.Height),
                    Mode = size.ResizeMode switch
                    {
                        "crop" => ResizeMode.Crop,
                        "fit" => ResizeMode.Max,
                        "stretch" => ResizeMode.Stretch,
                        _ => ResizeMode.Crop
                    }
                };
                
                resizedImage.Mutate(x => x.Resize(resizeOptions));
                
                // Add watermark if requested
                if (options.AddWatermark && !string.IsNullOrEmpty(options.WatermarkText))
                {
                    AddWatermark(resizedImage, options.WatermarkText);
                }
                
                // Save with appropriate format and compression
                var fileName = $"{fileNameWithoutExt}_{size.Name}.{options.Format}";
                var (url, fileSize) = await SaveImageVariantAsync(resizedImage, fileName, options);
                
                variants[size.Name] = new ProcessedImageVariant
                {
                    Url = url,
                    Width = resizedImage.Width,
                    Height = resizedImage.Height,
                    FileSize = fileSize,
                    Format = options.Format,
                    Usage = size.Usage
                };
                
                resizedImage.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating variant {SizeName} for {FileName}", size.Name, baseFileName);
                // Continue with other variants
            }
        }
        
        return variants;
    }

    private async Task<(string Url, long FileSize)> SaveImageVariantAsync(Image image, string fileName, ProductImageProcessingOptions options)
    {
        using var output = new MemoryStream();
        
        // Save with appropriate encoder
        switch (options.Format.ToLower())
        {
            case "webp":
                var webpEncoder = new WebpEncoder
                {
                    Quality = options.Quality,
                    Method = WebpEncodingMethod.BestQuality
                };
                await image.SaveAsync(output, webpEncoder);
                break;
                
            case "jpg":
            case "jpeg":
                var jpegEncoder = new JpegEncoder
                {
                    Quality = options.Quality
                };
                await image.SaveAsync(output, jpegEncoder);
                break;
                
            case "png":
                var pngEncoder = new PngEncoder
                {
                    CompressionLevel = PngCompressionLevel.BestCompression
                };
                await image.SaveAsync(output, pngEncoder);
                break;
                
            default:
                throw new NotSupportedException($"Format {options.Format} is not supported");
        }
        
        output.Position = 0;
        var url = await _fileStorage.SaveFileAsync(output, fileName, $"image/{options.Format}");
        
        return (url, output.Length);
    }

    public ImageMetadata ExtractMetadata(Image image)
    {
        return new ImageMetadata
        {
            OriginalWidth = image.Width,
            OriginalHeight = image.Height,
            ColorProfile = "sRGB", // Simplified for now
            HasTransparency = HasTransparency(image),
            DominantColor = DetectDominantColor(image),
            DetectedColors = ExtractColorPalette(image)
        };
    }

    public string DetectDominantColor(Image image)
    {
        // Simple dominant color detection
        using var resized = image.CloneAs<Rgba32>();
        resized.Mutate(x => x.Resize(1, 1));
        
        var pixel = resized[0, 0];
        return $"#{pixel.R:X2}{pixel.G:X2}{pixel.B:X2}";
    }

    private bool HasTransparency(Image image)
    {
        // Simplified transparency check
        return image.PixelType.BitsPerPixel == 32; // RGBA
    }

    private string[] ExtractColorPalette(Image image)
    {
        // Simplified color extraction
        var colors = new List<string>();
        
        using var resized = image.CloneAs<Rgba32>();
        resized.Mutate(x => x.Resize(50, 50)); // Sample colors from smaller image
        
        var colorCounts = new Dictionary<string, int>();
        
        for (int x = 0; x < resized.Width; x += 5)
        {
            for (int y = 0; y < resized.Height; y += 5)
            {
                var pixel = resized[x, y];
                var colorHex = $"#{pixel.R:X2}{pixel.G:X2}{pixel.B:X2}";
                
                colorCounts[colorHex] = colorCounts.GetValueOrDefault(colorHex, 0) + 1;
            }
        }
        
        return colorCounts
            .OrderByDescending(kvp => kvp.Value)
            .Take(5)
            .Select(kvp => kvp.Key)
            .ToArray();
    }

    private void AddWatermark(Image image, string watermarkText)
    {
        // Simplified watermark - requires font file
        try
        {
            var font = SystemFonts.CreateFont("Arial", 12);
            image.Mutate(x => x.DrawText(
                watermarkText,
                font,
                Color.FromRgba(255, 255, 255, 128),
                new PointF(10, image.Height - 30)
            ));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to add watermark: {Message}", ex.Message);
            // Continue without watermark if font not available
        }
    }
}

public class ProcessedImageResult
{
    public string MainUrl { get; set; } = null!;
    public string ThumbnailUrl { get; set; } = null!;
    public Dictionary<string, ProcessedImageVariant> Variants { get; set; } = new();
    public ImageMetadata Metadata { get; set; } = new();
    public TimeSpan ProcessingTime { get; set; }
    public long TotalFileSize { get; set; }
}
