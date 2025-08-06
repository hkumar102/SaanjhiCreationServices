using MediatR;
using RentalService.Contracts.Reports;
using RentalService.Infrastructure.Persistence;
using RentalService.Contracts.Enums;
using RentalService.Infrastructure.HttpClients;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using RentalService.Contracts.DTOs;
using AutoMapper;

namespace RentalService.Application.Reports.Rentals;

public class GetRentalDashboardReportQueryHandler : IRequestHandler<GetRentalDashboardReportQuery, RentalDashboardReportDto>
{
    private readonly RentalDbContext _dbContext;
    private readonly IProductApiClient _productApiClient;
    private readonly IMapper _mapper;

    public GetRentalDashboardReportQueryHandler(RentalDbContext dbContext, IProductApiClient productApiClient, IMapper mapper)
    {
        _dbContext = dbContext;
        _productApiClient = productApiClient;
        _mapper = mapper;
    }

    public async Task<RentalDashboardReportDto> Handle(GetRentalDashboardReportQuery request, CancellationToken cancellationToken)
    {
        var rentalsQuery = _dbContext.Rentals.AsNoTracking()
            .Where(r => r.Status == RentalStatus.Booked || r.Status == RentalStatus.PickedUp || r.Status == RentalStatus.Returned || r.Status == RentalStatus.Overdue);
        if (request.FromDate.HasValue)
            rentalsQuery = rentalsQuery.Where(r => r.BookingDate >= request.FromDate.Value.Date);
        if (request.ToDate.HasValue)
            rentalsQuery = rentalsQuery.Where(r => r.BookingDate <= request.ToDate.Value.Date);
        var rentals = await rentalsQuery.ToListAsync(cancellationToken);
        var totalEarning = rentals.Sum(r => r.RentalPrice);
        var totalSecurityDeposit = rentals.Sum(r => r.SecurityDeposit);
        var totalRentals = rentals.Count;
        var averageRentalPrice = totalRentals > 0 ? rentals.Average(r => r.RentalPrice) : 0;
        var productIds = rentals.Select(r => r.ProductId).Distinct().ToList();
        var products = await _productApiClient.GetProductsByIdsAsync(productIds, cancellationToken);
        var productLookup = products.ToDictionary(p => p.Id, p => p.Name);
        var listProductsGrouped = rentals
            .GroupBy(r => r.ProductId)
            .Select(g => new ProductRentalSummaryDto
            {
                ProductId = g.Key,
                ProductName = productLookup.GetValueOrDefault(g.Key) ?? "Unknown",
                TotalRentalAmount = g.Sum(r => r.RentalPrice),
                TotalRentalCount = g.Count()
            })
            .OrderByDescending(x => x.TotalRentalAmount)
            .ToList();
        var rentalDtos = _mapper.Map<List<RentalDto>>(rentals);
        return new RentalDashboardReportDto
        {
            TotalEarning = totalEarning,
            TotalSecurityDeposit = totalSecurityDeposit,
            TotalRentals = totalRentals,
            AverageRentalPrice = averageRentalPrice,
            ListOfRentals = rentalDtos,
            ListProductsGrouped = listProductsGrouped
        };
    }
}
