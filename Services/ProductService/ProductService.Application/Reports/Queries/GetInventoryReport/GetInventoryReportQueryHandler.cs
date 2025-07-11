using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Reports.Queries.GetInventoryReport;

public class GetInventoryReportQueryHandler(
    ProductDbContext db,
    ILogger<GetInventoryReportQueryHandler> logger)
    : IRequestHandler<GetInventoryReportQuery, List<InventoryReportDto>>
{
    public async Task<List<InventoryReportDto>> Handle(GetInventoryReportQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetInventoryReportQuery execution");

        try
        {
            // Build base query with joins
            var query = from inventory in db.InventoryItems
                        join product in db.Products on inventory.ProductId equals product.Id
                        select new { inventory, product };

            // Apply filters
            if (request.CategoryIds != null && request.CategoryIds.Any())
            {
                query = query.Where(x => request.CategoryIds.Contains(x.product.CategoryId));
            }

            if (request.Sizes != null && request.Sizes.Any())
            {
                query = query.Where(x => request.Sizes.Contains(x.inventory.Size));
            }

            if (request.Colors != null && request.Colors.Any())
            {
                query = query.Where(x => request.Colors.Contains(x.inventory.Color));
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x => x.inventory.Status == request.Status.Value);
            }

            if (request.Condition.HasValue)
            {
                query = query.Where(x => x.inventory.Condition == request.Condition.Value);
            }

            if (!request.IncludeRetired)
            {
                query = query.Where(x => !x.inventory.IsRetired);
            }

            if (request.FromDate.HasValue)
            {
                query = query.Where(x => x.inventory.CreatedAt >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(x => x.inventory.CreatedAt <= request.ToDate.Value);
            }

            logger.LogDebug("Executing inventory report query with filters applied");

            var reportData = await query
                .GroupBy(x => new 
                { 
                    x.product.Id, 
                    x.product.Name, 
                    x.product.CategoryId,
                    x.product.RentalPrice,
                    x.inventory.Size,
                    x.inventory.Color,
                    x.inventory.Status,
                    x.inventory.Condition
                })
                .Select(g => new InventoryReportDto
                {
                    ProductId = g.Key.Id,
                    ProductName = g.Key.Name,
                    CategoryId = g.Key.CategoryId,
                    Size = g.Key.Size,
                    Color = g.Key.Color,
                    Status = g.Key.Status,
                    Condition = g.Key.Condition,
                    Quantity = g.Count(),
                    TotalAcquisitionCost = g.Sum(x => x.inventory.AcquisitionCost),
                    AverageAcquisitionCost = g.Average(x => x.inventory.AcquisitionCost),
                    TotalRentalCount = g.Sum(x => x.inventory.TimesRented),
                    TotalRevenue = g.Sum(x => x.inventory.TimesRented * g.Key.RentalPrice),
                    OldestItemDate = g.Min(x => x.inventory.CreatedAt),
                    NewestItemDate = g.Max(x => x.inventory.CreatedAt)
                })
                .OrderBy(r => r.ProductName)
                .ThenBy(r => r.Size)
                .ThenBy(r => r.Color)
                .ToListAsync(cancellationToken);

            logger.LogDebug("GetInventoryReportQuery completed successfully. Generated {Count} report rows", reportData.Count);

            return reportData;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing GetInventoryReportQuery");
            throw;
        }
    }
}
