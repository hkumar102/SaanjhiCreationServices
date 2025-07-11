namespace ProductService.Contracts.DTOs;

/// <summary>
/// DTO for creating product media
/// </summary>
public class CreateProductMediaDto
{
    public Guid ProductId { get; set; }
    public string Url { get; set; } = null!;
    public string? PublicId { get; set; }
    public int MediaType { get; set; }
    public int SortOrder { get; set; } = 0;
    
    // Variant-Specific Properties
    public string? Color { get; set; }
    public string? Size { get; set; }
    public bool IsGeneric { get; set; } = false;
    
    // Media Classification
    public string? MediaPurpose { get; set; }
    public string? AltText { get; set; }
}

/// <summary>
/// DTO for updating product media
/// </summary>
public class UpdateProductMediaDto
{
    public string? Url { get; set; }
    public string? PublicId { get; set; }
    public int? MediaType { get; set; }
    public int? SortOrder { get; set; }
    
    // Variant-Specific Properties
    public string? Color { get; set; }
    public string? Size { get; set; }
    public bool? IsGeneric { get; set; }
    
    // Media Classification
    public string? MediaPurpose { get; set; }
    public string? AltText { get; set; }
}

/// <summary>
/// DTO for bulk media operations
/// </summary>
public class BulkMediaOperationDto
{
    public Guid ProductId { get; set; }
    public List<CreateProductMediaDto> MediaToAdd { get; set; } = new();
    public List<Guid> MediaToRemove { get; set; } = new();
    public List<UpdateProductMediaDto> MediaToUpdate { get; set; } = new();
}

/// <summary>
/// DTO for media query filters
/// </summary>
public class MediaQueryDto
{
    public Guid? ProductId { get; set; }
    public string? Color { get; set; }
    public string? Size { get; set; }
    public bool? IsGeneric { get; set; }
    public string? MediaPurpose { get; set; }
    public int? MediaType { get; set; }
}

/// <summary>
/// DTO for media organization by color/variant
/// </summary>
public class ProductMediaCollectionDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    
    // All media items
    public List<ProductMediaDto> AllMedia { get; set; } = new();
    
    // Organized collections
    public Dictionary<string, List<ProductMediaDto>> MediaByColor { get; set; } = new();
    public Dictionary<string, List<ProductMediaDto>> MediaByPurpose { get; set; } = new();
    public List<ProductMediaDto> GenericMedia { get; set; } = new();
    
    // Available colors with media
    public List<string> ColorsWithMedia { get; set; } = new();
    
    // Quick access
    public ProductMediaDto? MainImage { get; set; }
    public Dictionary<string, ProductMediaDto?> MainImageByColor { get; set; } = new();
}
