using System;

namespace RentalService.Contracts.Reports;

public class ProductRentalSummaryDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal TotalRentalAmount { get; set; }
    public int TotalRentalCount { get; set; }
}
