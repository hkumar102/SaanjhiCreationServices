namespace ProductService.Contracts.DTOs;

/// <summary>
/// DTO for Product - represents catalog/design information
/// </summary>
public class ProductDto
{
    public Guid Id { get; set; }
    
    // Basic Product Information
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Designer { get; set; }
    public string? SKU { get; set; }
    
    // Pricing Information
    public decimal PurchasePrice { get; set; }
    public decimal RentalPrice { get; set; }
    public decimal? SecurityDeposit { get; set; }
    public int MaxRentalDays { get; set; }
    
    // Product Specifications
    public string[] AvailableSizes { get; set; } = Array.Empty<string>();
    public string[] AvailableColors { get; set; } = Array.Empty<string>();
    public string? Material { get; set; }
    public string? CareInstructions { get; set; }
    public string[] Occasion { get; set; }
    public string? Season { get; set; }
    
    // Business Logic
    public bool IsActive { get; set; }
    public bool IsRentable { get; set; }
    public bool IsPurchasable { get; set; }
    
    // Category Information
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    
    // Related Data
    public List<ProductMediaDto> Media { get; set; } = new();
    public List<InventoryItemDto> InventoryItems { get; set; } = new();
    
    // Organized Media Collections
    public Dictionary<string, List<ProductMediaDto>> MediaByColor { get; set; } = new();
    public List<ProductMediaDto> GenericMedia { get; set; } = new();
    
    // Computed Properties
    public int TotalInventoryCount { get; set; }
    public int AvailableInventoryCount { get; set; }
    
    // Quick Access Properties
    public List<string> AvailableColorsWithMedia => MediaByColor.Keys.ToList();
    public ProductMediaDto? MainImage => Media.FirstOrDefault(m => m.MediaPurpose == "main") ?? Media.FirstOrDefault();
    
    // Audit Information
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
