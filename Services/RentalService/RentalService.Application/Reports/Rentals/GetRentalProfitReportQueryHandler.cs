using MediatR;
using RentalService.Contracts.Reports;
using RentalService.Infrastructure.Persistence;
using RentalService.Contracts.Enums;
using RentalService.Infrastructure.HttpClients;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace RentalService.Application.Reports.Rentals;

public class GetRentalProfitReportQueryHandler : IRequestHandler<GetRentalProfitReportQuery, RentalProfitReportDto>
{
    private readonly RentalDbContext _dbContext;
    private readonly IProductApiClient _productApiClient;

    public GetRentalProfitReportQueryHandler(RentalDbContext dbContext, IProductApiClient productApiClient)
    {
        _dbContext = dbContext;
        _productApiClient = productApiClient;
    }

    public async Task<RentalProfitReportDto> Handle(GetRentalProfitReportQuery request, CancellationToken cancellationToken)
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
            return new RentalProfitReportDto();

        // 2. Collect all needed InventoryItemIds
        var inventoryItemIds = rentals.Select(r => r.InventoryItemId).Distinct().ToList();

        // 3. Fetch all products (with inventory items) for these inventory items
        //    (Assume you have a method to get all products by inventory item IDs, or get all products for the period)
        //    For now, get all products for the productIds in rentals
        var productIds = rentals.Select(r => r.ProductId).Distinct().ToList();
        var products = await _productApiClient.GetProductsByIdsAsync(productIds, cancellationToken);

        // 4. Build inventory lookup: InventoryItemId -> InventoryItemDto
        var inventoryLookup = products
            .SelectMany(p => p.InventoryItems.Select(i => new { i.Id, InventoryItem = i }))
            .ToDictionary(x => x.Id, x => x.InventoryItem);

        // 5. Calculate profit for each rental
        var profitByProduct = new Dictionary<string, decimal>();
        var profitByCustomer = new Dictionary<string, decimal>();
        decimal totalProfit = 0;
        decimal totalRevenue = 0;
        decimal totalCost = 0;

        foreach (var rental in rentals)
        {
            if (!inventoryLookup.TryGetValue(rental.InventoryItemId, out var inventory))
                continue; // skip if inventory not found
            var profit = rental.RentalPrice - inventory.AcquisitionCost;
            totalProfit += profit;
            totalRevenue += rental.RentalPrice;
            totalCost += inventory.AcquisitionCost;

            // Breakdown by product
            var productKey = rental.ProductId.ToString();
            if (!profitByProduct.ContainsKey(productKey))
                profitByProduct[productKey] = 0;
            profitByProduct[productKey] += profit;

            // Breakdown by customer
            var customerKey = rental.CustomerId.ToString();
            if (!profitByCustomer.ContainsKey(customerKey))
                profitByCustomer[customerKey] = 0;
            profitByCustomer[customerKey] += profit;
        }

        return new RentalProfitReportDto
        {
            TotalProfit = totalProfit,
            TotalRevenue = totalRevenue,
            TotalCost = totalCost,
            ProfitByProduct = profitByProduct,
            ProfitByCategory = null, // Optional: implement if you want category breakdown
        };
    }
}
