using MediatR;
using RentalService.Contracts.Reports;
using RentalService.Infrastructure.Persistence;
using RentalService.Contracts.Enums;
using RentalService.Infrastructure.HttpClients;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace RentalService.Application.Reports.Rentals;

public class GetRentalsActivityReportQueryHandler : IRequestHandler<GetRentalsActivityReportQuery, RentalsActivityReportDto>
{
    private readonly RentalDbContext _dbContext;
    private readonly IProductApiClient _productApiClient;

    public GetRentalsActivityReportQueryHandler(RentalDbContext dbContext, IProductApiClient productApiClient)
    {
        _dbContext = dbContext;
        _productApiClient = productApiClient;
    }

    public async Task<RentalsActivityReportDto> Handle(GetRentalsActivityReportQuery request, CancellationToken cancellationToken)
    {
        // 1. Filter rentals: not Cancelled, by date/product
        var rentalsQuery = _dbContext.Rentals.AsNoTracking()
            .Where(r => r.Status != RentalStatus.Cancelled);

        if (request.FromDate.HasValue)
            rentalsQuery = rentalsQuery.Where(r => r.StartDate >= request.FromDate);
        if (request.ToDate.HasValue)
            rentalsQuery = rentalsQuery.Where(r => r.EndDate <= request.ToDate);
        // Optional: filter by product
        // if (request.ProductIds != null && request.ProductIds.Any())
        //     rentalsQuery = rentalsQuery.Where(r => request.ProductIds.Contains(r.ProductId));

        var rentals = await rentalsQuery.ToListAsync(cancellationToken);

        if (!rentals.Any())
            return new RentalsActivityReportDto();

        // 2. Build breakdowns by product and customer
        var rentalsByProduct = new Dictionary<string, int>();
        var rentalsByCustomer = new Dictionary<string, int>();

        int totalRentals = 0;
        int activeRentals = 0;
        int returnedRentals = 0;
        int overdueRentals = 0;
        int cancelledRentals = 0;

        foreach (var rental in rentals)
        {
            totalRentals++;
            switch (rental.Status)
            {
                case RentalStatus.Booked:
                case RentalStatus.PickedUp:
                    activeRentals++;
                    break;
                case RentalStatus.Returned:
                    returnedRentals++;
                    break;
                case RentalStatus.Overdue:
                    overdueRentals++;
                    break;
                case RentalStatus.Cancelled:
                    cancelledRentals++;
                    break;
            }

            // Breakdown by product
            var productKey = rental.ProductId.ToString();
            if (!rentalsByProduct.ContainsKey(productKey))
                rentalsByProduct[productKey] = 0;
            rentalsByProduct[productKey]++;

            // Breakdown by customer
            var customerKey = rental.CustomerId.ToString();
            if (!rentalsByCustomer.ContainsKey(customerKey))
                rentalsByCustomer[customerKey] = 0;
            rentalsByCustomer[customerKey]++;
        }

        return new RentalsActivityReportDto
        {
            TotalRentals = totalRentals,
            ActiveRentals = activeRentals,
            ReturnedRentals = returnedRentals,
            OverdueRentals = overdueRentals,
            CancelledRentals = cancelledRentals,
            RentalsByProduct = rentalsByProduct,
            RentalsByCustomer = rentalsByCustomer
        };
    }
}
