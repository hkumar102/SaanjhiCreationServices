using MediatR;
using RentalService.Contracts.Reports;

namespace RentalService.Application.Reports.Rentals;

public class GetRentalsActivityReportQuery : IRequest<RentalsActivityReportDto>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
