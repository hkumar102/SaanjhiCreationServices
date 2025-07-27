using MediatR;
using RentalService.Contracts.Reports;

namespace RentalService.Application.Reports.Rentals;

public class GetRentalRevenueReportQuery : IRequest<RentalRevenueReportDto>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
