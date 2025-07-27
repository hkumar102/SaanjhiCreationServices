using MediatR;
using RentalService.Contracts.Reports;

namespace RentalService.Application.Reports.Rentals;

public class GetRentalProfitReportQuery : IRequest<RentalProfitReportDto>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
