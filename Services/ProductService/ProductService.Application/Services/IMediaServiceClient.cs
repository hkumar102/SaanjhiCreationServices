using Microsoft.AspNetCore.Http;

namespace ProductService.Application.Services;

public interface IMediaServiceClient
{
    Task<MediaUploadResponse> UploadProductImageAsync(IFormFile file, bool isPrimary = false, string? color = null, string? altText = null);
}

public class MediaUploadResponse
{
    public Guid MediaId { get; set; }
    public string OriginalFileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long FileSize { get; set; }
    public string Url { get; set; } = null!;
    public string ThumbnailUrl { get; set; } = null!;
    public MediaVariants Variants { get; set; } = new();
    public MediaMetadata Metadata { get; set; } = new();
    public DateTime UploadedAt { get; set; }
    public string ProcessingStatus { get; set; } = null!;
}

public class MediaVariants
{
    public MediaVariant Thumbnail { get; set; } = new();
    public MediaVariant Small { get; set; } = new();
    public MediaVariant Medium { get; set; } = new();
    public MediaVariant Large { get; set; } = new();
    public MediaVariant Original { get; set; } = new();
}

public class MediaVariant
{
    public string Url { get; set; } = null!;
    public int Width { get; set; }
    public int Height { get; set; }
    public long FileSize { get; set; }
    public string Format { get; set; } = null!;
    public string Usage { get; set; } = null!;
}

public class MediaMetadata
{
    public int OriginalWidth { get; set; }
    public int OriginalHeight { get; set; }
    public string ColorProfile { get; set; } = null!;
    public bool HasTransparency { get; set; }
    public string DominantColor { get; set; } = null!;
    public string[] DetectedColors { get; set; } = Array.Empty<string>();
}
