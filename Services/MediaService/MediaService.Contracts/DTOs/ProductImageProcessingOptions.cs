namespace MediaService.Contracts.DTOs;

/// <summary>
/// Image processing configuration for product media
/// </summary>
public class ProductImageProcessingOptions
{
    // Standard sizes for clothing rental platform
    public ImageSize[] GenerateSizes { get; set; } = DefaultSizes;
    
    // Compression settings
    public int Quality { get; set; } = 85; // 1-100, 85 is good balance
    public string Format { get; set; } = "webp"; // webp, jpg, png
    
    // Processing flags
    public bool GenerateThumbnails { get; set; } = true;
    public bool OptimizeForWeb { get; set; } = true;
    public bool RemoveMetadata { get; set; } = true;
    public bool AutoOrient { get; set; } = true;
    
    // Watermark settings
    public bool AddWatermark { get; set; } = false;
    public string? WatermarkText { get; set; }
    
    // Default configuration for clothing rental
    public static ProductImageProcessingOptions Default => new()
    {
        GenerateSizes = DefaultSizes,
        Quality = 85,
        Format = "webp",
        GenerateThumbnails = true,
        OptimizeForWeb = true,
        RemoveMetadata = true,
        AutoOrient = true,
        AddWatermark = false
    };
    
    // Standard sizes for clothing rental platform
    public static ImageSize[] DefaultSizes => new[]
    {
        new ImageSize { Width = 150, Height = 150, Name = "thumbnail", Usage = "list_view" },
        new ImageSize { Width = 400, Height = 400, Name = "small", Usage = "grid_view" },
        new ImageSize { Width = 800, Height = 800, Name = "medium", Usage = "detail_view" },
        new ImageSize { Width = 1200, Height = 1200, Name = "large", Usage = "zoom_view" },
        new ImageSize { Width = 2000, Height = 2000, Name = "original", Usage = "print_quality" }
    };
}

public class ImageSize
{
    public int Width { get; set; }
    public int Height { get; set; }
    public string Name { get; set; } = null!;
    public string Usage { get; set; } = null!;
    public bool MaintainAspectRatio { get; set; } = true;
    public string ResizeMode { get; set; } = "crop"; // crop, fit, stretch
}

/// <summary>
/// Result of product media upload with all processed variants
/// </summary>
public class ProductMediaUploadResult
{
    public Guid MediaId { get; set; }
    public string OriginalFileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long FileSize { get; set; }
    
    // Main processed image
    public string Url { get; set; } = null!;
    public string ThumbnailUrl { get; set; } = null!;
    
    // All generated sizes
    public Dictionary<string, ProcessedImageVariant> Variants { get; set; } = new();
    
    // Metadata
    public ImageMetadata Metadata { get; set; } = new();
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    
    // Processing info
    public string ProcessingStatus { get; set; } = "completed";
    public TimeSpan ProcessingTime { get; set; }
}

public class ProcessedImageVariant
{
    public string Url { get; set; } = null!;
    public int Width { get; set; }
    public int Height { get; set; }
    public long FileSize { get; set; }
    public string Format { get; set; } = null!;
    public string Usage { get; set; } = null!;
}

public class ImageMetadata
{
    public int OriginalWidth { get; set; }
    public int OriginalHeight { get; set; }
    public string ColorProfile { get; set; } = null!;
    public bool HasTransparency { get; set; }
    public string DominantColor { get; set; } = null!;
    public string[] DetectedColors { get; set; } = Array.Empty<string>();
}
