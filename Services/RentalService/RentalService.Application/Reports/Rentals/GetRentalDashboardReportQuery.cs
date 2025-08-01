using System;
using MediatR;
using RentalService.Contracts.Reports;

namespace RentalService.Application.Reports.Rentals;

public class GetRentalDashboardReportQuery : IRequest<RentalDashboardReportDto>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
