using ProductService.Contracts.Enums;
using Shared.Domain.Entities;

namespace ProductService.Domain.Entities;

/// <summary>
/// InventoryItem represents the actual physical items that can be rented
/// Each Product can have multiple InventoryItems (different sizes, colors, individual units)
/// </summary>
public class InventoryItem : BaseEntity
{
    // Product Reference
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    // Physical Item Specifications
    public string Size { get; set; } = null!; // "M", "L", "XL", etc.
    public string Color { get; set; } = null!; // "Black", "Navy", etc.
    public string? SerialNumber { get; set; } // Unique identifier for this specific item
    public string? Barcode { get; set; } // For scanning and tracking

    // Item Status & Condition
    public InventoryStatus Status { get; set; } = InventoryStatus.Available;
    public ItemCondition Condition { get; set; } = ItemCondition.Excellent;
    public string? ConditionNotes { get; set; } // "Small stain on sleeve", "Missing button"

    // Business Metrics
    public int TimesRented { get; set; } = 0;
    public DateTime AcquisitionDate { get; set; }
    public decimal AcquisitionCost { get; set; }
    public DateTime? LastRentedDate { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }

    // Location & Logistics
    public string? WarehouseLocation { get; set; } // "Warehouse-A-Shelf-12"
    public string? StorageNotes { get; set; }

    // Maintenance & Lifecycle
    public DateTime? RetirementDate { get; set; } // When item is retired from service
    public string? RetirementReason { get; set; } // "Excessive wear", "Damaged beyond repair"
    public bool IsRetired { get; set; } = false;

    // Calculated Properties
    public bool IsAvailable => Status == InventoryStatus.Available && !IsRetired;
    public int DaysSinceLastRented => LastRentedDate.HasValue 
        ? (DateTime.UtcNow - LastRentedDate.Value).Days 
        : (DateTime.UtcNow - AcquisitionDate).Days;
}
