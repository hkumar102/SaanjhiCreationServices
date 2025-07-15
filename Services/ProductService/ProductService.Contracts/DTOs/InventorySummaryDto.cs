namespace ProductService.Contracts.DTOs;

/// <summary>
/// Summary DTO for inventory reporting and dashboards
/// </summary>
public class InventorySummaryDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string? Brand { get; set; }
    public string? SKU { get; set; }
    
    // Inventory Counts by Status
    public int TotalItems { get; set; }
    public int AvailableItems { get; set; }
    public int RentedItems { get; set; }
    public int MaintenanceItems { get; set; }
    public int RetiredItems { get; set; }
    
    // Inventory Counts by Condition
    public int NewItems { get; set; }
    public int ExcellentItems { get; set; }
    public int GoodItems { get; set; }
    public int FairItems { get; set; }
    public int PoorItems { get; set; }
    
    // Financial Summary
    public decimal TotalAcquisitionCost { get; set; }
    public decimal AverageAcquisitionCost { get; set; }
    public int TotalRentals { get; set; }
    
    // Last Activity
    public DateTime? LastRentedDate { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? OldestAcquisitionDate { get; set; }
    public DateTime? NewestAcquisitionDate { get; set; }
}
