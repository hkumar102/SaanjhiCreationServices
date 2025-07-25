using ProductService.Contracts.Enums;

namespace ProductService.Contracts.DTOs;

/// <summary>
/// DTO for InventoryItem - represents physical items in inventory
/// </summary>
public class InventoryItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }

    // Physical Item Properties
    public string Size { get; set; } = null!;
    public string Color { get; set; } = null!;
    public string? SerialNumber { get; set; }
    public string? BarcodeImageBase64 { get; set; }
    public string? QRCodeImageBase64 { get; set; } // QR code image as Base64 string


    // Item Status and Condition
    public InventoryStatus Status { get; set; }
    public ItemCondition Condition { get; set; }
    public string? ConditionNotes { get; set; }

    // Business Metrics
    public int TimesRented { get; set; }
    public DateTime AcquisitionDate { get; set; }
    public decimal AcquisitionCost { get; set; }
    public DateTime? LastRentedDate { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }

    // Location & Logistics
    public string? WarehouseLocation { get; set; }
    public string? StorageNotes { get; set; }

    // Retirement Information
    public DateTime? RetirementDate { get; set; }
    public string? RetirementReason { get; set; }
    public bool IsRetired { get; set; }

    // Related Information
    public string ProductName { get; set; } = null!;
    public string? ProductBrand { get; set; }

    // Computed Properties
    public bool IsAvailable { get; set; }
    public int DaysSinceLastRented { get; set; }

    public string[] AvailableSizes { get; set; } = Array.Empty<string>();
    public string[] AvailableColors { get; set; } = Array.Empty<string>();
    public List<ProductMediaDto> Media { get; set; } = new();

    // Audit Information
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public decimal PurchasePrice { get; set; }
    public decimal RentalPrice { get; set; }
    public decimal? SecurityDeposit { get; set; }
}
