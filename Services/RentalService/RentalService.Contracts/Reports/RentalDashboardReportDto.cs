using System;
using System.Collections.Generic;
using RentalService.Contracts.DTOs;

namespace RentalService.Contracts.Reports;

public class RentalDashboardReportDto
{
    public decimal TotalEarning { get; set; }
    public decimal TotalSecurityDeposit { get; set; }
    public int TotalRentals { get; set; }
    public decimal AverageRentalPrice { get; set; }
    public List<RentalDto> ListOfRentals { get; set; } = new();
    public List<ProductRentalSummaryDto> ListProductsGrouped { get; set; } = new();
}
