using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Reports.Queries.GetDemandAnalysis;

public class GetDemandAnalysisQueryHandler(
    ProductDbContext db,
    ILogger<GetDemandAnalysisQueryHandler> logger)
    : IRequestHandler<GetDemandAnalysisQuery, List<DemandAnalysisReportDto>>
{
    public async Task<List<DemandAnalysisReportDto>> Handle(GetDemandAnalysisQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetDemandAnalysisQuery execution");

        try
        {
            var fromDate = request.FromDate ?? DateTime.UtcNow.AddMonths(-6);
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

            var results = new List<DemandAnalysisReportDto>();

            if (request.AnalysisType == "size" || request.AnalysisType == "all")
            {
                var sizeAnalysis = await query
                    .GroupBy(i => i.Size)
                    .Select(g => new DemandAnalysisReportDto
                    {
                        DimensionType = "Size",
                        DimensionValue = g.Key,
                        TotalRentals = g.Count(),
                        TotalRevenue = g.Sum(i => i.Product.RentalPrice),
                        AverageRentalPrice = g.Average(i => i.Product.RentalPrice),
                        UniqueProducts = g.Select(i => i.ProductId).Distinct().Count(),
                        UtilizationRate = (double)g.Count() / g.Select(i => i.Id).Distinct().Count() * 100,
                        MarketShare = 0 // Will be calculated after retrieval
                    })
                    .OrderByDescending(r => r.TotalRentals)
                    .ToListAsync(cancellationToken);

                var totalSizeRentals = sizeAnalysis.Sum(s => s.TotalRentals);
                foreach (var item in sizeAnalysis)
                {
                    item.MarketShare = totalSizeRentals > 0 ? (double)item.TotalRentals / totalSizeRentals * 100 : 0;
                }
                results.AddRange(sizeAnalysis);
            }

            if (request.AnalysisType == "color" || request.AnalysisType == "all")
            {
                var colorAnalysis = await query
                    .GroupBy(i => i.Color)
                    .Select(g => new DemandAnalysisReportDto
                    {
                        DimensionType = "Color",
                        DimensionValue = g.Key,
                        TotalRentals = g.Count(),
                        TotalRevenue = g.Sum(i => i.Product.RentalPrice),
                        AverageRentalPrice = g.Average(i => i.Product.RentalPrice),
                        UniqueProducts = g.Select(i => i.ProductId).Distinct().Count(),
                        UtilizationRate = (double)g.Count() / g.Select(i => i.Id).Distinct().Count() * 100,
                        MarketShare = 0
                    })
                    .OrderByDescending(r => r.TotalRentals)
                    .ToListAsync(cancellationToken);

                var totalColorRentals = colorAnalysis.Sum(c => c.TotalRentals);
                foreach (var item in colorAnalysis)
                {
                    item.MarketShare = totalColorRentals > 0 ? (double)item.TotalRentals / totalColorRentals * 100 : 0;
                }
                results.AddRange(colorAnalysis);
            }

            if (request.AnalysisType == "category" || request.AnalysisType == "all")
            {
                var categoryAnalysis = await query
                    .GroupBy(i => new { i.Product.CategoryId, i.Product.Category!.Name })
                    .Select(g => new DemandAnalysisReportDto
                    {
                        DimensionType = "Category",
                        DimensionValue = g.Key.Name,
                        TotalRentals = g.Count(),
                        TotalRevenue = g.Sum(i => i.Product.RentalPrice),
                        AverageRentalPrice = g.Average(i => i.Product.RentalPrice),
                        UniqueProducts = g.Select(i => i.ProductId).Distinct().Count(),
                        UtilizationRate = (double)g.Count() / g.Select(i => i.Id).Distinct().Count() * 100,
                        MarketShare = 0
                    })
                    .OrderByDescending(r => r.TotalRentals)
                    .ToListAsync(cancellationToken);

                var totalCategoryRentals = categoryAnalysis.Sum(c => c.TotalRentals);
                foreach (var item in categoryAnalysis)
                {
                    item.MarketShare = totalCategoryRentals > 0 ? (double)item.TotalRentals / totalCategoryRentals * 100 : 0;
                }
                results.AddRange(categoryAnalysis);
            }

            logger.LogDebug("Retrieved {ResultCount} demand analysis records", results.Count);
            return results;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing GetDemandAnalysisQuery");
            throw;
        }
    }
}
