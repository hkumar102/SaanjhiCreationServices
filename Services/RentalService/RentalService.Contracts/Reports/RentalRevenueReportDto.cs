namespace RentalService.Contracts.Reports;

public class RentalRevenueReportDto
{
    public decimal TotalRevenue { get; set; }
    public Dictionary<string, decimal>? RevenueByCategory { get; set; }
    public Dictionary<string, decimal>? RevenueByProduct { get; set; }
    public int RentalCount { get; set; }
}
