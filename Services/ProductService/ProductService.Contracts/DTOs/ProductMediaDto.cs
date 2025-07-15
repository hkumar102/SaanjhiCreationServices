namespace ProductService.Contracts.DTOs;

/// <summary>
/// DTO for ProductMedia with enhanced MediaService integration
/// </summary>
public class ProductMediaDto
{
    public Guid ProductId { get; set; }
    public Guid Id { get; set; }
    public Guid MediaId { get; set; }  // From MediaService
    
    // Essential URLs
    public string Url { get; set; } = null!;
    public string ThumbnailUrl { get; set; } = null!;
    
    // Display management
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
    
    // Image metadata
    public string OriginalFileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long FileSize { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    
    // User metadata
    public string? AltText { get; set; }
    public string? Color { get; set; }
    
    // Processing info
    public DateTime UploadedAt { get; set; }
    public string ProcessingStatus { get; set; } = null!;
    
    // Variants (parsed from JSON)
    public MediaVariantsDto? Variants { get; set; }
    
    // Legacy fields for backward compatibility
    public string? PublicId { get; set; }
    public int MediaType { get; set; }
    public int SortOrder { get; set; }
    public string? Size { get; set; }
    public bool IsGeneric { get; set; }
    public string? MediaPurpose { get; set; }
}

public class MediaVariantsDto
{
    public MediaVariantDto? Thumbnail { get; set; }
    public MediaVariantDto? Small { get; set; }
    public MediaVariantDto? Medium { get; set; }
    public MediaVariantDto? Large { get; set; }
    public MediaVariantDto? Original { get; set; }
}

public class MediaVariantDto
{
    public string Url { get; set; } = null!;
    public int Width { get; set; }
    public int Height { get; set; }
    public long FileSize { get; set; }
    public string Format { get; set; } = null!;
    public string Usage { get; set; } = null!;
}
