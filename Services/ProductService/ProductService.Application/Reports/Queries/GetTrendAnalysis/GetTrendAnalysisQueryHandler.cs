using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Reports.Queries.GetTrendAnalysis;

public class GetTrendAnalysisQueryHandler(
    ProductDbContext db,
    ILogger<GetTrendAnalysisQueryHandler> logger)
    : IRequestHandler<GetTrendAnalysisQuery, List<TrendAnalysisReportDto>>
{
    public async Task<List<TrendAnalysisReportDto>> Handle(GetTrendAnalysisQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetTrendAnalysisQuery execution");

        try
        {
            var fromDate = request.FromDate ?? DateTime.UtcNow.AddMonths(-12);
            var toDate = request.ToDate ?? DateTime.UtcNow;

            var query = db.InventoryItems
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .Where(i => i.LastRentedDate.HasValue && 
                           i.LastRentedDate >= fromDate && 
                           i.LastRentedDate <= toDate)
                .AsQueryable();

            // Apply category filter
            if (request.CategoryIds?.Any() == true)
            {
                query = query.Where(i => request.CategoryIds.Contains(i.Product.CategoryId));
            }

            var inventoryItems = await query.ToListAsync(cancellationToken);

            var results = inventoryItems
                .GroupBy(i => new 
                { 
                    i.ProductId,
                    ProductName = i.Product.Name,
                    i.Product.CategoryId,
                    CategoryName = i.Product.Category?.Name,
                    Period = GetPeriodFromDate(i.LastRentedDate!.Value, request.GroupBy)
                })
                .Select(g => new TrendAnalysisReportDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    Period = g.Key.Period,
                    RentalCount = g.Count(),
                    TotalRevenue = g.Sum(i => i.Product.RentalPrice),
                    AverageRentalPrice = g.Average(i => i.Product.RentalPrice),
                    UniqueCustomers = g.Select(i => i.Id).Distinct().Count() // Using item count as proxy for unique rentals
                })
                .OrderBy(r => r.Period)
                .ThenByDescending(r => r.RentalCount)
                .ToList();

            logger.LogDebug("Retrieved {ResultCount} trend analysis records", results.Count);
            return results;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing GetTrendAnalysisQuery");
            throw;
        }
    }

    private static DateTime GetPeriodFromDate(DateTime date, string? groupBy)
    {
        return groupBy?.ToLower() switch
        {
            "day" => date.Date,
            "week" => date.Date.AddDays(-(int)date.DayOfWeek),
            "quarter" => new DateTime(date.Year, ((date.Month - 1) / 3) * 3 + 1, 1),
            _ => new DateTime(date.Year, date.Month, 1)
        };
    }
}
