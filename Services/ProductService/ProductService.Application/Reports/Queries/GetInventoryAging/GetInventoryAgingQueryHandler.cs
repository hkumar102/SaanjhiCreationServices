using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Contracts.Enums;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Reports.Queries.GetInventoryAging;

public class GetInventoryAgingQueryHandler(
    ProductDbContext db,
    ILogger<GetInventoryAgingQueryHandler> logger)
    : IRequestHandler<GetInventoryAgingQuery, List<InventoryAgingReportDto>>
{
    public async Task<List<InventoryAgingReportDto>> Handle(GetInventoryAgingQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetInventoryAgingQuery execution");

        try
        {
            var query = db.InventoryItems
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .AsQueryable();

            // Apply filters
            if (request.CategoryIds?.Any() == true)
            {
                query = query.Where(i => request.CategoryIds.Contains(i.Product.CategoryId));
            }

            if (request.Sizes?.Any() == true)
            {
                query = query.Where(i => request.Sizes.Contains(i.Size));
            }

            if (request.Colors?.Any() == true)
            {
                query = query.Where(i => request.Colors.Contains(i.Color));
            }

            if (!request.IncludeRetired)
            {
                query = query.Where(i => !i.IsRetired);
            }

            var inventoryItems = await query
                .Select(i => new InventoryAgingReportDto
                {
                    InventoryItemId = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Brand = i.Product.Brand,
                    Size = i.Size,
                    Color = i.Color,
                    SerialNumber = i.SerialNumber,
                    Condition = i.Condition,
                    Status = i.Status,
                    AcquisitionDate = i.AcquisitionDate,
                    DaysSinceAcquisition = (int)(DateTime.UtcNow - i.AcquisitionDate).TotalDays,
                    LastRentalDate = i.LastRentedDate,
                    DaysSinceLastRental = i.LastRentedDate.HasValue 
                        ? (int)(DateTime.UtcNow - i.LastRentedDate.Value).TotalDays 
                        : (int)(DateTime.UtcNow - i.AcquisitionDate).TotalDays,
                    LastMaintenanceDate = i.LastMaintenanceDate,
                    DaysSinceLastMaintenance = i.LastMaintenanceDate.HasValue 
                        ? (int)(DateTime.UtcNow - i.LastMaintenanceDate.Value).TotalDays 
                        : (int)(DateTime.UtcNow - i.AcquisitionDate).TotalDays,
                    TimesRented = i.TimesRented,
                    AcquisitionCost = i.AcquisitionCost,
                    CurrentValue = CalculateCurrentValue(i.AcquisitionCost, i.Condition),
                    AgingCategory = CalculateAgingCategory((int)(DateTime.UtcNow - i.AcquisitionDate).TotalDays),
                    IsLowActivity = i.LastRentedDate == null || 
                        (DateTime.UtcNow - i.LastRentedDate.Value).TotalDays > request.LowActivityThresholdDays,
                    NeedsMaintenance = i.LastMaintenanceDate == null || 
                        (DateTime.UtcNow - i.LastMaintenanceDate.Value).TotalDays > request.MaintenanceThresholdDays
                })
                .ToListAsync(cancellationToken);

            logger.LogDebug("Retrieved {ItemCount} inventory items for aging analysis", inventoryItems.Count);
            return inventoryItems;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing GetInventoryAgingQuery");
            throw;
        }
    }

    private static decimal CalculateCurrentValue(decimal acquisitionCost, ItemCondition condition)
    {
        return condition switch
        {
            ItemCondition.New => acquisitionCost,
            ItemCondition.Excellent => acquisitionCost * 0.9m,
            ItemCondition.Good => acquisitionCost * 0.75m,
            ItemCondition.Fair => acquisitionCost * 0.5m,
            ItemCondition.Poor => acquisitionCost * 0.25m,
            _ => acquisitionCost
        };
    }

    private static string CalculateAgingCategory(int daysSinceAcquisition)
    {
        return daysSinceAcquisition switch
        {
            <= 30 => "0-30 days",
            <= 90 => "31-90 days",
            <= 180 => "91-180 days",
            _ => "180+ days"
        };
    }
}
