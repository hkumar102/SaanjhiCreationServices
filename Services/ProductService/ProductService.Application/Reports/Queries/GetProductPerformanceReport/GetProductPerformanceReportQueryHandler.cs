using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Reports.Queries.GetProductPerformanceReport;

public class GetProductPerformanceReportQueryHandler(
    ProductDbContext db,
    ILogger<GetProductPerformanceReportQueryHandler> logger)
    : IRequestHandler<GetProductPerformanceReportQuery, List<ProductPerformanceReportDto>>
{
    public async Task<List<ProductPerformanceReportDto>> Handle(GetProductPerformanceReportQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetProductPerformanceReportQuery execution");

        try
        {
            // Build base query with joins
            var query = from product in db.Products
                        join inventory in db.InventoryItems on product.Id equals inventory.ProductId into inventoryItems
                        from inventory in inventoryItems.DefaultIfEmpty()
                        select new { product, inventory };

            // Apply filters
            if (request.CategoryIds != null && request.CategoryIds.Any())
            {
                query = query.Where(x => request.CategoryIds.Contains(x.product.CategoryId));
            }

            if (request.FromDate.HasValue)
            {
                query = query.Where(x => x.inventory == null || x.inventory.CreatedAt >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(x => x.inventory == null || x.inventory.CreatedAt <= request.ToDate.Value);
            }

            logger.LogDebug("Executing product performance report query with filters applied");

            var reportData = await query
                .GroupBy(x => new { x.product.Id, x.product.Name, x.product.CategoryId, x.product.RentalPrice, x.product.PurchasePrice })
                .Select(g => new ProductPerformanceReportDto
                {
                    ProductId = g.Key.Id,
                    ProductName = g.Key.Name,
                    RentalPrice = g.Key.RentalPrice,
                    TotalInventoryItems = g.Count(x => x.inventory != null),
                    AvailableItems = g.Count(x => x.inventory != null && 
                                                   x.inventory.Status == Contracts.Enums.InventoryStatus.Available && 
                                                   !x.inventory.IsRetired),
                    RentedItems = g.Count(x => x.inventory != null && 
                                                x.inventory.Status == Contracts.Enums.InventoryStatus.Rented),
                    TotalRentals = g.Sum(x => x.inventory != null ? x.inventory.TimesRented : 0),
                    TotalRentalRevenue = g.Sum(x => x.inventory != null ? (x.inventory.TimesRented * g.Key.RentalPrice) : 0),
                    AverageRentalPrice = g.Key.RentalPrice,
                    UtilizationRate = g.Where(x => x.inventory != null && x.inventory.TimesRented > 0).Count() > 0 
                        ? (double)g.Count(x => x.inventory != null && x.inventory.Status == Contracts.Enums.InventoryStatus.Rented) / 
                          g.Count(x => x.inventory != null) * 100 : 0,
                    TurnoverRate = g.Where(x => x.inventory != null).Any() 
                        ? (double)g.Sum(x => x.inventory != null ? x.inventory.TimesRented : 0) / 
                          Math.Max(g.Count(x => x.inventory != null), 1) : 0,
                    TotalAcquisitionCost = g.Sum(x => x.inventory != null ? x.inventory.AcquisitionCost : 0),
                    ROI = 0, // Will be calculated after query
                    ReportDate = DateTime.UtcNow
                })
                .OrderByDescending(r => r.TotalRentalRevenue)
                .Take(request.TopProducts)
                .ToListAsync(cancellationToken);

            // Calculate additional metrics
            foreach (var report in reportData)
            {
                if (report.TotalAcquisitionCost > 0)
                {
                    report.ROI = (report.TotalRentalRevenue - report.TotalAcquisitionCost) / report.TotalAcquisitionCost * 100;
                }

                // Calculate days to break even
                if (report.AverageRentalPrice > 0)
                {
                    report.DaysToBreakEven = (int)Math.Ceiling(report.TotalAcquisitionCost / report.AverageRentalPrice);
                }
            }

            logger.LogDebug("GetProductPerformanceReportQuery completed successfully. Generated {Count} report rows", reportData.Count);

            return reportData;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing GetProductPerformanceReportQuery");
            throw;
        }
    }
}
