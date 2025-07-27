namespace RentalService.Contracts.Reports;

public class RentalProfitReportDto
{
    public decimal TotalProfit { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalCost { get; set; }
    public Dictionary<string, decimal>? ProfitByCategory { get; set; }
    public Dictionary<string, decimal>? ProfitByProduct { get; set; }
}
