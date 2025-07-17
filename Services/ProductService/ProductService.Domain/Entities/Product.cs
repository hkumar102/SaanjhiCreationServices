using Shared.Domain.Entities;

namespace ProductService.Domain.Entities;

/// <summary>
/// Product represents the catalog/design template - what customers see and browse
/// </summary>
public class Product : BaseEntity
{
    // Basic Product Information
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Designer { get; set; }
    public string? SKU { get; set; } // Stock Keeping Unit for product line

    // Pricing Information
    public decimal PurchasePrice { get; set; } // Price to buy the item
    public decimal RentalPrice { get; set; } // Base rental price per day
    public decimal? SecurityDeposit { get; set; }
    public int MaxRentalDays { get; set; } = 7; // Default 7 days

    // Product Specifications
    public string[] AvailableSizes { get; set; } = Array.Empty<string>(); // ["XS", "S", "M", "L", "XL"]
    public string[] AvailableColors { get; set; } = Array.Empty<string>(); // ["Black", "Navy", "Red"]
    public string? Material { get; set; } // "Silk", "Cotton", "Polyester"
    public string? CareInstructions { get; set; }
    public string[] Occasion { get; set; } = Array.Empty<string>();// "Wedding", "Formal", "Casual"
    public string? Season { get; set; } // "Spring/Summer", "Fall/Winter", "All Season"

    // Business Logic
    public bool IsActive { get; set; } = true;
    public bool IsRentable { get; set; } = true;
    public bool IsPurchasable { get; set; } = false;

    // Relationships
    public Guid CategoryId { get; set; }

    // Navigation Properties
    public Category Category { get; set; } = null!;
    public ICollection<ProductMedia> Media { get; set; } = new List<ProductMedia>();
    public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
}
