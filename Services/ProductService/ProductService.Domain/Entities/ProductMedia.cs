using Shared.Contracts.Media;
using Shared.Domain.Entities;

namespace ProductService.Domain.Entities;

public class ProductMedia : BaseEntity
{
    // Core relationship
    public Guid ProductId { get; set; }
    public Guid MediaId { get; set; }  // From MediaService response
    
    // Essential URLs for performance
    public string Url { get; set; } = null!;          // Main image URL (medium variant)
    public string ThumbnailUrl { get; set; } = null!;  // For listings/grids
    
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
    public string ProcessingStatus { get; set; } = "completed";
    
    // Full variants for flexibility (JSON)
    public string? VariantsJson { get; set; }
    
    // Legacy fields for backward compatibility
    public string? PublicId { get; set; }
    public MediaType MediaType { get; set; } = MediaType.Image;
    public int SortOrder { get; set; } = 0;
    public string? Size { get; set; }
    public bool IsGeneric { get; set; } = false;
    public string? MediaPurpose { get; set; }
    
    // Navigation Property
    public Product Product { get; set; } = null!;
}
