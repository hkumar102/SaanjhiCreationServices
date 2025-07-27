namespace RentalService.Contracts.Reports;

public class RentalsActivityReportDto
{
    public int TotalRentals { get; set; }
    public int ActiveRentals { get; set; }
    public int ReturnedRentals { get; set; }
    public int OverdueRentals { get; set; }
    public int CancelledRentals { get; set; }
    public Dictionary<string, int>? RentalsByProduct { get; set; }
    public Dictionary<string, int>? RentalsByCustomer { get; set; }
}
