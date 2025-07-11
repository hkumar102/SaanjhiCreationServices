using Shared.Contracts.Media;
using Shared.Domain.Entities;

namespace ProductService.Domain.Entities;

public class ProductMedia : BaseEntity
{
    public Guid ProductId { get; set; }
    public string Url { get; set; } = null!;
    public string? PublicId { get; set; }
    public MediaType MediaType { get; set; }
    public int SortOrder { get; set; } = 0;
    
    // Variant-Specific Properties
    public string? Color { get; set; }        // "Black", "Red", null for all colors
    public string? Size { get; set; }         // "XL", null for all sizes  
    public bool IsGeneric { get; set; } = false; // Generic images (size chart, care instructions)
    
    // Media Classification
    public string? MediaPurpose { get; set; } // "main", "detail", "back", "lifestyle", "size_chart"
    public string? AltText { get; set; }      // Accessibility description
    
    // Navigation Property
    public Product Product { get; set; } = null!;
}
