using System.ComponentModel.DataAnnotations;

namespace ProductService.Contracts.DTOs;

/// <summary>
/// DTO for creating product media with validation
/// </summary>
public class CreateProductMediaValidationDto
{
    [Required]
    public Guid ProductId { get; set; }
    
    [Required]
    [Url]
    [StringLength(500)]
    public string Url { get; set; } = null!;
    
    [StringLength(200)]
    public string? PublicId { get; set; }
    
    [Required]
    [Range(1, 10)] // Assuming MediaType enum values 1-10
    public int MediaType { get; set; }
    
    [Range(0, 999)]
    public int SortOrder { get; set; } = 0;
    
    // Variant-Specific Properties
    [StringLength(50)]
    public string? Color { get; set; }
    
    [StringLength(20)]
    public string? Size { get; set; }
    
    public bool IsGeneric { get; set; } = false;
    
    // Media Classification
    [StringLength(50)]
    [RegularExpression("^(main|detail|back|lifestyle|size_chart|care_instructions|fit_guide)$", 
        ErrorMessage = "MediaPurpose must be one of: main, detail, back, lifestyle, size_chart, care_instructions, fit_guide")]
    public string? MediaPurpose { get; set; }
    
    [StringLength(500)]
    public string? AltText { get; set; }
}

/// <summary>
/// DTO for updating product media with validation
/// </summary>
public class UpdateProductMediaValidationDto
{
    [Url]
    [StringLength(500)]
    public string? Url { get; set; }
    
    [StringLength(200)]
    public string? PublicId { get; set; }
    
    [Range(1, 10)]
    public int? MediaType { get; set; }
    
    [Range(0, 999)]
    public int? SortOrder { get; set; }
    
    // Variant-Specific Properties
    [StringLength(50)]
    public string? Color { get; set; }
    
    [StringLength(20)]
    public string? Size { get; set; }
    
    public bool? IsGeneric { get; set; }
    
    // Media Classification
    [StringLength(50)]
    [RegularExpression("^(main|detail|back|lifestyle|size_chart|care_instructions|fit_guide)$", 
        ErrorMessage = "MediaPurpose must be one of: main, detail, back, lifestyle, size_chart, care_instructions, fit_guide")]
    public string? MediaPurpose { get; set; }
    
    [StringLength(500)]
    public string? AltText { get; set; }
}

/// <summary>
/// DTO for media operation results
/// </summary>
public class MediaOperationResultDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();
    public ProductMediaDto? Media { get; set; }
    public List<ProductMediaDto> AffectedMedia { get; set; } = new();
}

/// <summary>
/// DTO for media validation rules
/// </summary>
public class MediaValidationRuleDto
{
    public int MaxMediaPerProduct { get; set; } = 20;
    public int MaxMediaPerColor { get; set; } = 10;
    public List<string> AllowedMediaPurposes { get; set; } = new()
    {
        "main", "detail", "back", "lifestyle", "size_chart", "care_instructions", "fit_guide"
    };
    public List<string> AllowedFileExtensions { get; set; } = new()
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };
    public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024; // 5MB
    public bool RequireAltText { get; set; } = true;
    public bool RequireMainImagePerColor { get; set; } = true;
}
