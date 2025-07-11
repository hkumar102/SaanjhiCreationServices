using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.Reports.Queries.GetInventoryAging;
using ProductService.Application.Reports.Queries.GetProductPerformanceReport;
using ProductService.Contracts.DTOs;
using ProductService.Contracts.Enums;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Application.Reports.Queries.GetDashboardSummary;

public class GetDashboardSummaryQueryHandler(
    ProductDbContext db,
    IMediator mediator,
    ILogger<GetDashboardSummaryQueryHandler> logger)
    : IRequestHandler<GetDashboardSummaryQuery, InventoryDashboardDto>
{
    public async Task<InventoryDashboardDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetDashboardSummaryQuery execution");

        try
        {
            var fromDate = request.FromDate ?? DateTime.UtcNow.AddMonths(-1);
            var toDate = request.ToDate ?? DateTime.UtcNow;

            // Get basic inventory statistics
            var inventoryStats = await GetInventoryStatistics(cancellationToken);
            
            // Get financial metrics
            var financialMetrics = await GetFinancialMetrics(fromDate, toDate, cancellationToken);
            
            // Get performance data
            var performanceData = await mediator.Send(new GetProductPerformanceReportQuery(), cancellationToken);
            
            // Get maintenance alerts
            var maintenanceAlerts = await mediator.Send(new GetInventoryAgingQuery(), cancellationToken);
            
            // Get recent activity
            var recentActivity = await GetRecentActivity(fromDate, toDate, cancellationToken);

            var dashboard = new InventoryDashboardDto
            {
                // Inventory Stats
                TotalProducts = inventoryStats.TotalProducts,
                TotalInventoryItems = inventoryStats.TotalInventoryItems,
                AvailableItems = inventoryStats.AvailableItems,
                RentedItems = inventoryStats.RentedItems,
                MaintenanceItems = inventoryStats.MaintenanceItems,
                RetiredItems = inventoryStats.RetiredItems,
                
                // Financial Summary
                TotalInventoryValue = financialMetrics.TotalInventoryValue,
                MonthlyRentalRevenue = financialMetrics.MonthlyRentalRevenue,
                AverageItemValue = financialMetrics.AverageItemValue,
                
                // Performance Metrics
                OverallUtilizationRate = CalculateUtilizationRate(inventoryStats.RentedItems, inventoryStats.TotalInventoryItems - inventoryStats.RetiredItems),
                LowStockProducts = await GetLowStockProductCount(cancellationToken),
                LowActivityItems = maintenanceAlerts.Count(a => a.IsLowActivity),
                MaintenanceOverdueItems = maintenanceAlerts.Count(a => a.NeedsMaintenance),
                
                // Recent Activity
                NewItemsThisMonth = recentActivity.NewItemsThisMonth,
                RetiredItemsThisMonth = recentActivity.RetiredItemsThisMonth,
                
                // Top performing products (top 5)
                TopPerformingProducts = performanceData
                    .OrderByDescending(p => p.TotalRentalRevenue)
                    .Take(5)
                    .ToList(),
                    
                // Maintenance alerts (items needing attention)
                MaintenanceAlerts = maintenanceAlerts
                    .Where(a => a.NeedsMaintenance || a.IsLowActivity)
                    .OrderByDescending(a => a.DaysSinceLastMaintenance)
                    .Take(10)
                    .ToList()
            };

            logger.LogDebug("Generated dashboard summary with {ProductCount} products and {ItemCount} inventory items", 
                dashboard.TotalProducts, dashboard.TotalInventoryItems);
                
            return dashboard;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing GetDashboardSummaryQuery");
            throw;
        }
    }

    private async Task<(int TotalProducts, int TotalInventoryItems, int AvailableItems, int RentedItems, int MaintenanceItems, int RetiredItems)> 
        GetInventoryStatistics(CancellationToken cancellationToken)
    {
        var totalProducts = await db.Products.CountAsync(cancellationToken);
        
        var inventoryStats = await db.InventoryItems
            .GroupBy(i => 1) // Group all items together
            .Select(g => new
            {
                TotalItems = g.Count(),
                AvailableItems = g.Count(i => i.Status == InventoryStatus.Available),
                RentedItems = g.Count(i => i.Status == InventoryStatus.Rented),
                MaintenanceItems = g.Count(i => i.Status == InventoryStatus.Maintenance),
                RetiredItems = g.Count(i => i.Status == InventoryStatus.Retired)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return (
            totalProducts,
            inventoryStats?.TotalItems ?? 0,
            inventoryStats?.AvailableItems ?? 0,
            inventoryStats?.RentedItems ?? 0,
            inventoryStats?.MaintenanceItems ?? 0,
            inventoryStats?.RetiredItems ?? 0
        );
    }

    private async Task<(decimal TotalInventoryValue, decimal MonthlyRentalRevenue, decimal AverageItemValue)> 
        GetFinancialMetrics(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        var totalInventoryValue = await db.InventoryItems
            .Where(i => !i.IsRetired)
            .SumAsync(i => i.AcquisitionCost, cancellationToken);

        var averageItemValue = await db.InventoryItems
            .Where(i => !i.IsRetired)
            .AverageAsync(i => (decimal?)i.AcquisitionCost, cancellationToken) ?? 0;

        // For monthly rental revenue, we'd normally query rental transactions
        // Since we don't have that service here, we'll calculate based on estimated revenue
        var estimatedMonthlyRevenue = await db.Products
            .Include(p => p.InventoryItems)
            .Where(p => p.InventoryItems.Any(i => i.Status == InventoryStatus.Rented))
            .SumAsync(p => p.RentalPrice * p.InventoryItems.Count(i => i.Status == InventoryStatus.Rented), cancellationToken);

        return (totalInventoryValue, estimatedMonthlyRevenue, averageItemValue);
    }

    private async Task<(int NewItemsThisMonth, int RetiredItemsThisMonth)> 
        GetRecentActivity(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
    {
        var newItemsThisMonth = await db.InventoryItems
            .CountAsync(i => i.AcquisitionDate >= fromDate && i.AcquisitionDate <= toDate, cancellationToken);

        var retiredItemsThisMonth = await db.InventoryItems
            .CountAsync(i => i.IsRetired && i.ModifiedAt >= fromDate && i.ModifiedAt <= toDate, cancellationToken);

        return (newItemsThisMonth, retiredItemsThisMonth);
    }

    private async Task<int> GetLowStockProductCount(CancellationToken cancellationToken)
    {
        return await db.Products
            .CountAsync(p => p.InventoryItems.Count(i => i.Status == InventoryStatus.Available) < 2, cancellationToken);
    }

    private static double CalculateUtilizationRate(int rentedItems, int activeItems)
    {
        return activeItems > 0 ? (double)rentedItems / activeItems * 100 : 0;
    }
}
