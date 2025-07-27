using MediatR;
using RentalService.Contracts.Reports;
using RentalService.Infrastructure.Persistence;
using RentalService.Contracts.Enums;
using RentalService.Infrastructure.HttpClients;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace RentalService.Application.Reports.Rentals;

public class GetRentalRevenueReportQueryHandler : IRequestHandler<GetRentalRevenueReportQuery, RentalRevenueReportDto>
{
    private readonly RentalDbContext _dbContext;
    private readonly IProductApiClient _productApiClient;

    public GetRentalRevenueReportQueryHandler(RentalDbContext dbContext, IProductApiClient productApiClient)
    {
        _dbContext = dbContext;
        _productApiClient = productApiClient;
    }

    public async Task<RentalRevenueReportDto> Handle(GetRentalRevenueReportQuery request, CancellationToken cancellationToken)
    {
        // 1. Filter rentals: Returned, not Cancelled, by date/product
        var rentalsQuery = _dbContext.Rentals.AsNoTracking()
            .Where(r => r.Status == RentalStatus.Returned);

        if (request.FromDate.HasValue)
            rentalsQuery = rentalsQuery.Where(r => r.StartDate >= request.FromDate);
        if (request.ToDate.HasValue)
            rentalsQuery = rentalsQuery.Where(r => r.EndDate <= request.ToDate);
        // Optional: filter by product
        // if (request.ProductIds != null && request.ProductIds.Any())
        //     rentalsQuery = rentalsQuery.Where(r => request.ProductIds.Contains(r.ProductId));

        var rentals = await rentalsQuery.ToListAsync(cancellationToken);

        if (!rentals.Any())
            return new RentalRevenueReportDto();

        // 2. Collect all needed ProductIds
        var productIds = rentals.Select(r => r.ProductId).Distinct().ToList();
        var products = await _productApiClient.GetProductsByIdsAsync(productIds, cancellationToken);

        // 3. Build product lookup: ProductId -> ProductDto
        var productLookup = products.ToDictionary(p => p.Id, p => p);

        // 4. Calculate revenue for each rental
        var revenueByProduct = new Dictionary<string, decimal>();
        var revenueByCustomer = new Dictionary<string, decimal>();
        decimal totalRevenue = 0;
        int rentalCount = 0;

        foreach (var rental in rentals)
        {
            totalRevenue += rental.RentalPrice;
            rentalCount++;

            // Breakdown by product
            var productKey = rental.ProductId.ToString();
            if (!revenueByProduct.ContainsKey(productKey))
                revenueByProduct[productKey] = 0;
            revenueByProduct[productKey] += rental.RentalPrice;

            // Breakdown by customer
            var customerKey = rental.CustomerId.ToString();
            if (!revenueByCustomer.ContainsKey(customerKey))
                revenueByCustomer[customerKey] = 0;
            revenueByCustomer[customerKey] += rental.RentalPrice;
        }

        return new RentalRevenueReportDto
        {
            TotalRevenue = totalRevenue,
            RevenueByProduct = revenueByProduct,
            RevenueByCategory = null, // Optional: implement if you want category breakdown
            RentalCount = rentalCount
        };
    }
}
