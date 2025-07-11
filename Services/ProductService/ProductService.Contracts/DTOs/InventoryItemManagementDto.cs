using ProductService.Contracts.Enums;

namespace ProductService.Contracts.DTOs;

/// <summary>
/// DTO for creating or updating inventory items
/// </summary>
public class CreateInventoryItemDto
{
    public Guid ProductId { get; set; }
    
    // Physical Item Properties
    public string Size { get; set; } = null!;
    public string Color { get; set; } = null!;
    public string? SerialNumber { get; set; }
    public string? Barcode { get; set; }
    
    // Initial Status and Condition
    public InventoryStatus Status { get; set; } = InventoryStatus.Available;
    public ItemCondition Condition { get; set; } = ItemCondition.New;
    public string? ConditionNotes { get; set; }
    
    // Acquisition Information
    public decimal AcquisitionCost { get; set; }
    public DateTime AcquisitionDate { get; set; } = DateTime.UtcNow;
    
    // Location Information
    public string? WarehouseLocation { get; set; }
    public string? StorageNotes { get; set; }
}

/// <summary>
/// DTO for updating inventory item details
/// </summary>
public class UpdateInventoryItemDto
{
    public string? Size { get; set; }
    public string? Color { get; set; }
    public string? SerialNumber { get; set; }
    public string? Barcode { get; set; }
    
    // Status and Condition Updates
    public InventoryStatus? Status { get; set; }
    public ItemCondition? Condition { get; set; }
    public string? ConditionNotes { get; set; }
    
    // Acquisition Updates
    public decimal? AcquisitionCost { get; set; }
    public DateTime? AcquisitionDate { get; set; }
    
    // Location Updates
    public string? WarehouseLocation { get; set; }
    public string? StorageNotes { get; set; }
    
    // Lifecycle Updates
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? RetirementDate { get; set; }
    public string? RetirementReason { get; set; }
    public bool? IsRetired { get; set; }
}

/// <summary>
/// DTO for inventory item status updates (used by RentalService)
/// </summary>
public class InventoryItemStatusUpdateDto
{
    public Guid InventoryItemId { get; set; }
    public InventoryStatus Status { get; set; }
    public ItemCondition? Condition { get; set; }
    public DateTime? LastRentedDate { get; set; }
    public string? ConditionNotes { get; set; }
}
