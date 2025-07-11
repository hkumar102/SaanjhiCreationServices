using ProductService.Contracts.Enums;

namespace ProductService.Contracts.DTOs;

/// <summary>
/// DTO for basic inventory report
/// </summary>
public class InventoryReportDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string Size { get; set; } = null!;
    public string Color { get; set; } = null!;
    public InventoryStatus Status { get; set; }
    public ItemCondition Condition { get; set; }
    
    // Quantity and Cost Metrics
    public int Quantity { get; set; }
    public decimal TotalAcquisitionCost { get; set; }
    public decimal AverageAcquisitionCost { get; set; }
    
    // Rental Metrics
    public int TotalRentalCount { get; set; }
    public decimal TotalRevenue { get; set; }
    
    // Date Information
    public DateTime OldestItemDate { get; set; }
    public DateTime NewestItemDate { get; set; }
}

/// <summary>
/// DTO for inventory aging report
/// </summary>
public class InventoryAgingReportDto
{
    public Guid InventoryItemId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string? Brand { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public string? SerialNumber { get; set; }
    
    public ItemCondition Condition { get; set; }
    public InventoryStatus Status { get; set; }
    
    public DateTime AcquisitionDate { get; set; }
    public int DaysSinceAcquisition { get; set; }
    public decimal AcquisitionCost { get; set; }
    
    public int TimesRented { get; set; }
    public DateTime? LastRentalDate { get; set; }
    public int? DaysSinceLastRental { get; set; }
    
    public DateTime? LastMaintenanceDate { get; set; }
    public int? DaysSinceLastMaintenance { get; set; }
    
    public decimal CurrentValue { get; set; }
    
    // Aging Categories
    public string AgingCategory { get; set; } = null!; // "0-30 days", "31-90 days", "91-180 days", "180+ days"
    public bool IsLowActivity { get; set; } // True if not rented in last 90 days
    public bool NeedsMaintenance { get; set; } // True if no maintenance in last 180 days
}

/// <summary>
/// DTO for product performance report
/// </summary>
public class ProductPerformanceReportDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string? Brand { get; set; }
    public string? CategoryName { get; set; }
    public decimal RentalPrice { get; set; }
    
    // Inventory Summary
    public int TotalInventoryItems { get; set; }
    public int AvailableItems { get; set; }
    public int RentedItems { get; set; }
    
    // Performance Metrics
    public int TotalRentals { get; set; }
    public decimal TotalRentalRevenue { get; set; }
    public decimal AverageRentalPrice { get; set; }
    public double UtilizationRate { get; set; } // Percentage of time items are rented
    public double TurnoverRate { get; set; } // Rentals per item per month
    
    // Financial Metrics
    public decimal TotalAcquisitionCost { get; set; }
    public decimal ROI { get; set; } // Return on Investment
    public int DaysToBreakEven { get; set; }
    
    // Date Ranges
    public DateTime? FirstRentalDate { get; set; }
    public DateTime? LastRentalDate { get; set; }
    public DateTime ReportDate { get; set; }
}

/// <summary>
/// DTO for inventory valuation report
/// </summary>
public class InventoryValuationReportDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string? Brand { get; set; }
    public string? CategoryName { get; set; }
    
    // Inventory Breakdown by Condition
    public Dictionary<ItemCondition, int> ItemsByCondition { get; set; } = new();
    public Dictionary<ItemCondition, decimal> ValueByCondition { get; set; } = new();
    
    // Financial Summary
    public decimal TotalAcquisitionCost { get; set; }
    public decimal CurrentMarketValue { get; set; } // Estimated based on condition
    public decimal DepreciationAmount { get; set; }
    public double DepreciationRate { get; set; }
    
    // Lifecycle Summary
    public int TotalItems { get; set; }
    public int ActiveItems { get; set; }
    public int RetiredItems { get; set; }
    public DateTime? OldestAcquisition { get; set; }
    public DateTime? NewestAcquisition { get; set; }
}

/// <summary>
/// DTO for dashboard summary statistics
/// </summary>
public class InventoryDashboardDto
{
    // Overall Inventory Stats
    public int TotalProducts { get; set; }
    public int TotalInventoryItems { get; set; }
    public int AvailableItems { get; set; }
    public int RentedItems { get; set; }
    public int MaintenanceItems { get; set; }
    public int RetiredItems { get; set; }
    
    // Financial Summary
    public decimal TotalInventoryValue { get; set; }
    public decimal MonthlyRentalRevenue { get; set; }
    public decimal AverageItemValue { get; set; }
    
    // Performance Metrics
    public double OverallUtilizationRate { get; set; }
    public int LowStockProducts { get; set; } // Products with < 2 available items
    public int LowActivityItems { get; set; } // Items not rented in 90 days
    public int MaintenanceOverdueItems { get; set; } // Items needing maintenance
    
    // Recent Activity
    public int NewItemsThisMonth { get; set; }
    public int RetiredItemsThisMonth { get; set; }
    public List<ProductPerformanceReportDto> TopPerformingProducts { get; set; } = new();
    public List<InventoryAgingReportDto> MaintenanceAlerts { get; set; } = new();
}

/// <summary>
/// DTO for trend analysis report
/// </summary>
public class TrendAnalysisReportDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public DateTime Period { get; set; }
    public int RentalCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageRentalPrice { get; set; }
    public int UniqueCustomers { get; set; }
}

/// <summary>
/// DTO for demand analysis report (size, color, category analysis)
/// </summary>
public class DemandAnalysisReportDto
{
    public string DimensionType { get; set; } = null!; // "Size", "Color", "Category"
    public string DimensionValue { get; set; } = null!;
    public int TotalRentals { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageRentalPrice { get; set; }
    public int UniqueProducts { get; set; }
    public double UtilizationRate { get; set; }
    public double MarketShare { get; set; } // Percentage of total rentals
}
