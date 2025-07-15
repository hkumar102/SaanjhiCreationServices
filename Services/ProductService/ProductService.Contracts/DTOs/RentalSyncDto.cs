using ProductService.Contracts.Enums;

namespace ProductService.Contracts.DTOs;

/// <summary>
/// DTO for synchronizing product and inventory data with RentalService
/// Contains all necessary information for rental operations
/// </summary>
public class ProductRentalSyncDto
{
    // Product Information
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string? Brand { get; set; }
    public string? Designer { get; set; }
    public string? SKU { get; set; }
    
    // Rental Configuration
    public decimal RentalPrice { get; set; }
    public decimal? SecurityDeposit { get; set; }
    public int MaxRentalDays { get; set; }
    public bool IsRentable { get; set; }
    public bool IsActive { get; set; }
    
    // Category Information
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    
    // Inventory Items Available for Rental
    public List<InventoryItemRentalDto> AvailableInventoryItems { get; set; } = new();
    
    // Product Specifications (for rental matching)
    public string[] AvailableSizes { get; set; } = Array.Empty<string>();
    public string[] AvailableColors { get; set; } = Array.Empty<string>();
    public string? Material { get; set; }
    public string? CareInstructions { get; set; }
    public string? Occasion { get; set; }
    
    // Sync Information
    public DateTime LastUpdated { get; set; }
    public int Version { get; set; } // For optimistic concurrency
}

/// <summary>
/// DTO for inventory item information needed by RentalService
/// </summary>
public class InventoryItemRentalDto
{
    public Guid InventoryItemId { get; set; }
    public Guid ProductId { get; set; }
    
    // Physical Properties
    public string Size { get; set; } = null!;
    public string Color { get; set; } = null!;
    public string? SerialNumber { get; set; }
    public string? Barcode { get; set; }
    
    // Status Information
    public InventoryStatus Status { get; set; }
    public ItemCondition Condition { get; set; }
    public string? ConditionNotes { get; set; }
    public bool IsRetired { get; set; }
    
    // Rental History
    public int TimesRented { get; set; }
    public DateTime? LastRentedDate { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    
    // For availability calculations
    public DateTime? AvailableFrom { get; set; }
    public DateTime? ReservedUntil { get; set; }
}

/// <summary>
/// DTO for bulk inventory status updates from RentalService
/// </summary>
public class BulkInventoryStatusUpdateDto
{
    public List<InventoryItemStatusUpdateDto> Updates { get; set; } = new();
    public DateTime UpdateTimestamp { get; set; } = DateTime.UtcNow;
    public string? BatchId { get; set; } // For tracking bulk operations
    public string? Source { get; set; } = "RentalService"; // Source of the update
}

/// <summary>
/// DTO for responding to rental availability queries
/// </summary>
public class RentalAvailabilityResponseDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public bool IsAvailable { get; set; }
    public int AvailableCount { get; set; }
    
    // Available items grouped by size/color
    public Dictionary<string, List<InventoryItemRentalDto>> AvailableBySize { get; set; } = new();
    public Dictionary<string, List<InventoryItemRentalDto>> AvailableByColor { get; set; } = new();
    
    // Pricing Information
    public decimal RentalPrice { get; set; }
    public decimal? SecurityDeposit { get; set; }
    public int MaxRentalDays { get; set; }
    
    // Next Availability
    public DateTime? NextAvailableDate { get; set; }
    public List<DateTime> UpcomingAvailableDates { get; set; } = new();
}
