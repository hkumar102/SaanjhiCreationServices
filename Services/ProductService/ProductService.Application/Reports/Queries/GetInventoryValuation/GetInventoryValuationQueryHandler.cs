using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Contracts.Enums;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Reports.Queries.GetInventoryValuation;

public class GetInventoryValuationQueryHandler(
    ProductDbContext db,
    ILogger<GetInventoryValuationQueryHandler> logger)
    : IRequestHandler<GetInventoryValuationQuery, List<InventoryValuationReportDto>>
{
    public async Task<List<InventoryValuationReportDto>> Handle(GetInventoryValuationQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetInventoryValuationQuery execution");

        try
        {
            var asOfDate = request.AsOfDate ?? DateTime.UtcNow;
            
            var query = db.Products
                .Include(p => p.Category)
                .Include(p => p.InventoryItems)
                .AsQueryable();

            // Apply filters
            if (request.CategoryIds?.Any() == true)
            {
                query = query.Where(p => request.CategoryIds.Contains(p.CategoryId));
            }

            if (request.ProductIds?.Any() == true)
            {
                query = query.Where(p => request.ProductIds.Contains(p.Id));
            }

            var products = await query.ToListAsync(cancellationToken);

            var valuationReports = products.Select(product =>
            {
                var inventoryItems = product.InventoryItems
                    .Where(i => request.IncludeRetired || !i.IsRetired)
                    .Where(i => i.AcquisitionDate <= asOfDate)
                    .ToList();

                if (!inventoryItems.Any())
                    return null;

                var itemsByCondition = inventoryItems
                    .GroupBy(i => i.Condition)
                    .ToDictionary(g => g.Key, g => g.Count());

                var valueByCondition = inventoryItems
                    .GroupBy(i => i.Condition)
                    .ToDictionary(g => g.Key, g => g.Sum(i => CalculateCurrentValue(i.AcquisitionCost, i.Condition, asOfDate - i.AcquisitionDate)));

                var totalAcquisitionCost = inventoryItems.Sum(i => i.AcquisitionCost);
                var currentMarketValue = inventoryItems.Sum(i => CalculateCurrentValue(i.AcquisitionCost, i.Condition, asOfDate - i.AcquisitionDate));
                var depreciationAmount = totalAcquisitionCost - currentMarketValue;
                var depreciationRate = totalAcquisitionCost > 0 ? (double)(depreciationAmount / totalAcquisitionCost) : 0;

                return new InventoryValuationReportDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Brand = product.Brand,
                    CategoryName = product.Category?.Name,
                    ItemsByCondition = itemsByCondition,
                    ValueByCondition = valueByCondition,
                    TotalAcquisitionCost = totalAcquisitionCost,
                    CurrentMarketValue = currentMarketValue,
                    DepreciationAmount = depreciationAmount,
                    DepreciationRate = depreciationRate,
                    TotalItems = inventoryItems.Count,
                    ActiveItems = inventoryItems.Count(i => !i.IsRetired),
                    RetiredItems = inventoryItems.Count(i => i.IsRetired),
                    OldestAcquisition = inventoryItems.Min(i => i.AcquisitionDate),
                    NewestAcquisition = inventoryItems.Max(i => i.AcquisitionDate)
                };
            })
            .Where(r => r != null)
            .Cast<InventoryValuationReportDto>()
            .ToList();

            logger.LogDebug("Generated valuation report for {ProductCount} products", valuationReports.Count);
            return valuationReports;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing GetInventoryValuationQuery");
            throw;
        }
    }

    private static decimal CalculateCurrentValue(decimal acquisitionCost, ItemCondition condition, TimeSpan age)
    {
        // Base depreciation by condition
        var conditionMultiplier = condition switch
        {
            ItemCondition.New => 1.0m,
            ItemCondition.Excellent => 0.9m,
            ItemCondition.Good => 0.75m,
            ItemCondition.Fair => 0.5m,
            ItemCondition.Poor => 0.25m,
            _ => 1.0m
        };

        // Additional depreciation based on age (clothing typically depreciates over time)
        var ageDays = (int)age.TotalDays;
        var ageMultiplier = ageDays switch
        {
            <= 30 => 1.0m,      // No age depreciation in first month
            <= 90 => 0.95m,     // 5% depreciation by 3 months
            <= 180 => 0.9m,     // 10% depreciation by 6 months
            <= 365 => 0.85m,    // 15% depreciation by 1 year
            _ => 0.8m           // 20% depreciation after 1 year
        };

        return acquisitionCost * conditionMultiplier * ageMultiplier;
    }
}
