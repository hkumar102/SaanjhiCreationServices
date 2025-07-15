using ProductService.Contracts.Enums;

namespace ProductService.Contracts.DTOs;

/// <summary>
/// Base class for all product service events
/// </summary>
public abstract class ProductServiceEventDto
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = null!;
    public int Version { get; set; } = 1;
}

/// <summary>
/// Event published when a product is created
/// </summary>
public class ProductCreatedEventDto : ProductServiceEventDto
{
    public ProductCreatedEventDto()
    {
        EventType = "ProductCreated";
    }
    
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string? Brand { get; set; }
    public string? SKU { get; set; }
    public Guid CategoryId { get; set; }
    public decimal RentalPrice { get; set; }
    public decimal? SecurityDeposit { get; set; }
    public int MaxRentalDays { get; set; }
    public bool IsRentable { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Event published when a product is updated
/// </summary>
public class ProductUpdatedEventDto : ProductServiceEventDto
{
    public ProductUpdatedEventDto()
    {
        EventType = "ProductUpdated";
    }
    
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string? Brand { get; set; }
    public string? SKU { get; set; }
    public Guid CategoryId { get; set; }
    public decimal RentalPrice { get; set; }
    public decimal? SecurityDeposit { get; set; }
    public int MaxRentalDays { get; set; }
    public bool IsRentable { get; set; }
    public bool IsActive { get; set; }
    
    // What fields were changed
    public List<string> ChangedFields { get; set; } = new();
}

/// <summary>
/// Event published when inventory item is created
/// </summary>
public class InventoryItemCreatedEventDto : ProductServiceEventDto
{
    public InventoryItemCreatedEventDto()
    {
        EventType = "InventoryItemCreated";
    }
    
    public Guid InventoryItemId { get; set; }
    public Guid ProductId { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public string? SerialNumber { get; set; }
    public InventoryStatus Status { get; set; }
    public ItemCondition Condition { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Event published when inventory item status changes
/// </summary>
public class InventoryItemStatusChangedEventDto : ProductServiceEventDto
{
    public InventoryItemStatusChangedEventDto()
    {
        EventType = "InventoryItemStatusChanged";
    }
    
    public Guid InventoryItemId { get; set; }
    public Guid ProductId { get; set; }
    public InventoryStatus OldStatus { get; set; }
    public InventoryStatus NewStatus { get; set; }
    public ItemCondition? OldCondition { get; set; }
    public ItemCondition? NewCondition { get; set; }
    public string? Reason { get; set; }
    public string? UpdatedBy { get; set; }
}

/// <summary>
/// Event published when inventory item is retired
/// </summary>
public class InventoryItemRetiredEventDto : ProductServiceEventDto
{
    public InventoryItemRetiredEventDto()
    {
        EventType = "InventoryItemRetired";
    }
    
    public Guid InventoryItemId { get; set; }
    public Guid ProductId { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime RetirementDate { get; set; }
    public string? RetirementReason { get; set; }
    public int TotalRentals { get; set; }
    public decimal AcquisitionCost { get; set; }
}

/// <summary>
/// Event published when product availability changes
/// </summary>
public class ProductAvailabilityChangedEventDto : ProductServiceEventDto
{
    public ProductAvailabilityChangedEventDto()
    {
        EventType = "ProductAvailabilityChanged";
    }
    
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public int PreviousAvailableCount { get; set; }
    public int CurrentAvailableCount { get; set; }
    public bool IsNowUnavailable { get; set; }
    public bool IsNowAvailable { get; set; }
    
    // Breakdown by size/color if applicable
    public Dictionary<string, int> AvailabilityBySize { get; set; } = new();
    public Dictionary<string, int> AvailabilityByColor { get; set; } = new();
}
