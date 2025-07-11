namespace ProductService.Contracts.DTOs;

/// <summary>
/// DTO for ProductMedia with color/variant support
/// </summary>
public class ProductMediaDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = null!;
    public string? PublicId { get; set; }
    public int MediaType { get; set; }
    public int SortOrder { get; set; }
    
    // Variant-Specific Properties
    public string? Color { get; set; }        // "Black", "Red", null for all colors
    public string? Size { get; set; }         // "XL", null for all sizes  
    public bool IsGeneric { get; set; }       // Generic images (size chart, care instructions)
    
    // Media Classification
    public string? MediaPurpose { get; set; } // "main", "detail", "back", "lifestyle", "size_chart"
    public string? AltText { get; set; }      // Accessibility description
}
