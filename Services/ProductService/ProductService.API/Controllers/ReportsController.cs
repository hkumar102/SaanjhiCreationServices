using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Reports.Queries.GetInventoryReport;
using ProductService.Application.Reports.Queries.GetProductPerformanceReport;
using ProductService.Application.Reports.Queries.GetInventoryAging;
using ProductService.Application.Reports.Queries.GetInventoryValuation;
using ProductService.Application.Reports.Queries.GetDashboardSummary;
using ProductService.Application.Reports.Queries.GetTrendAnalysis;
using ProductService.Application.Reports.Queries.GetDemandAnalysis;
using ProductService.Contracts.DTOs;

namespace ProductService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get inventory report with stock levels, condition summaries, and aging analysis
    /// </summary>
    /// <param name="categoryIds">Filter by category IDs (optional)</param>
    /// <param name="includeRetired">Include retired items in report (optional)</param>
    /// <returns>Inventory report data</returns>
    [HttpGet("inventory")]
    public async Task<ActionResult<List<InventoryReportDto>>> GetInventoryReport(
        [FromQuery] List<Guid>? categoryIds = null,
        [FromQuery] bool includeRetired = false)
    {
        var query = new GetInventoryReportQuery(categoryIds, null, null, null, null, includeRetired);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get product performance report with rental statistics and revenue data
    /// </summary>
    /// <param name="categoryIds">Filter by category IDs (optional)</param>
    /// <returns>Product performance report data</returns>
    [HttpGet("product-performance")]
    public async Task<ActionResult<List<ProductPerformanceReportDto>>> GetProductPerformanceReport(
        [FromQuery] List<Guid>? categoryIds = null)
    {
        var query = new GetProductPerformanceReportQuery(categoryIds);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get summary statistics across all products and inventory
    /// </summary>
    /// <returns>Summary report with key metrics</returns>
    [HttpGet("summary")]
    public async Task<ActionResult<object>> GetSummaryReport()
    {
        // Get both reports and combine into summary
        var inventoryTask = mediator.Send(new GetInventoryReportQuery());
        var performanceTask = mediator.Send(new GetProductPerformanceReportQuery());
        
        await Task.WhenAll(inventoryTask, performanceTask);
        
        var inventoryReport = await inventoryTask;
        var performanceReport = await performanceTask;
        
        var summary = new
        {
            TotalProducts = inventoryReport.Select(i => i.ProductId).Distinct().Count(),
            TotalInventoryItems = inventoryReport.Sum(i => i.Quantity),
            AvailableItems = inventoryReport.Where(i => i.Status == ProductService.Contracts.Enums.InventoryStatus.Available).Sum(i => i.Quantity),
            RentedItems = inventoryReport.Where(i => i.Status == ProductService.Contracts.Enums.InventoryStatus.Rented).Sum(i => i.Quantity),
            MaintenanceItems = inventoryReport.Where(i => i.Status == ProductService.Contracts.Enums.InventoryStatus.Maintenance).Sum(i => i.Quantity),
            RetiredItems = inventoryReport.Where(i => i.Status == ProductService.Contracts.Enums.InventoryStatus.Retired).Sum(i => i.Quantity),
            TotalRevenue = performanceReport.Sum(p => p.TotalRentalRevenue),
            AverageRentalPrice = performanceReport.Where(p => p.AverageRentalPrice > 0).DefaultIfEmpty().Average(p => p?.AverageRentalPrice ?? 0),
            TopPerformingProducts = performanceReport.OrderByDescending(p => p.TotalRentalRevenue).Take(5),
            GeneratedAt = DateTime.UtcNow
        };
        
        return Ok(summary);
    }

    /// <summary>
    /// Get low stock alert report for products with inventory below thresholds
    /// </summary>
    /// <param name="minimumAvailable">Minimum available items threshold (default: 2)</param>
    /// <returns>Products with low stock levels</returns>
    [HttpGet("low-stock")]
    public async Task<ActionResult<object>> GetLowStockReport([FromQuery] int minimumAvailable = 2)
    {
        var inventoryReport = await mediator.Send(new GetInventoryReportQuery());
        
        // Group by product and calculate available quantities
        var productSummaries = inventoryReport
            .GroupBy(i => new { i.ProductId, i.ProductName, i.CategoryName })
            .Select(g => new
            {
                g.Key.ProductId,
                g.Key.ProductName,
                g.Key.CategoryName,
                AvailableCount = g.Where(i => i.Status == ProductService.Contracts.Enums.InventoryStatus.Available).Sum(i => i.Quantity),
                TotalCount = g.Sum(i => i.Quantity),
                RentedCount = g.Where(i => i.Status == ProductService.Contracts.Enums.InventoryStatus.Rented).Sum(i => i.Quantity)
            })
            .Where(p => p.AvailableCount < minimumAvailable)
            .Select(p => new
            {
                p.ProductId,
                p.ProductName,
                p.CategoryName,
                p.AvailableCount,
                p.TotalCount,
                p.RentedCount,
                AlertLevel = p.AvailableCount == 0 ? "Critical" : "Warning"
            })
            .OrderBy(p => p.AvailableCount)
            .ToList();
        
        return Ok(new
        {
            AlertDate = DateTime.UtcNow,
            MinimumThreshold = minimumAvailable,
            TotalAlertsCount = productSummaries.Count,
            CriticalAlerts = productSummaries.Count(p => p.AlertLevel == "Critical"),
            WarningAlerts = productSummaries.Count(p => p.AlertLevel == "Warning"),
            Products = productSummaries
        });
    }

    /// <summary>
    /// Get comprehensive dashboard with key business metrics
    /// </summary>
    /// <param name="fromDate">Start date for metrics calculation (optional)</param>
    /// <param name="toDate">End date for metrics calculation (optional)</param>
    /// <returns>Dashboard with key metrics and alerts</returns>
    [HttpGet("dashboard")]
    public async Task<ActionResult<InventoryDashboardDto>> GetDashboard(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var query = new GetDashboardSummaryQuery(fromDate, toDate);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get inventory aging report showing maintenance needs and low activity items
    /// </summary>
    /// <param name="categoryIds">Filter by category IDs (optional)</param>
    /// <param name="maintenanceThresholdDays">Days threshold for maintenance alerts (default: 180)</param>
    /// <param name="lowActivityThresholdDays">Days threshold for low activity alerts (default: 90)</param>
    /// <param name="includeRetired">Include retired items (optional)</param>
    /// <returns>Inventory aging analysis</returns>
    [HttpGet("aging")]
    public async Task<ActionResult<List<InventoryAgingReportDto>>> GetInventoryAging(
        [FromQuery] List<Guid>? categoryIds = null,
        [FromQuery] int maintenanceThresholdDays = 180,
        [FromQuery] int lowActivityThresholdDays = 90,
        [FromQuery] bool includeRetired = false)
    {
        var query = new GetInventoryAgingQuery(
            categoryIds, 
            null, 
            null, 
            maintenanceThresholdDays, 
            lowActivityThresholdDays, 
            includeRetired);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get inventory valuation report with depreciation analysis
    /// </summary>
    /// <param name="categoryIds">Filter by category IDs (optional)</param>
    /// <param name="productIds">Filter by specific product IDs (optional)</param>
    /// <param name="includeRetired">Include retired items (optional)</param>
    /// <param name="asOfDate">Valuation as of specific date (optional, defaults to current date)</param>
    /// <returns>Inventory valuation analysis</returns>
    [HttpGet("valuation")]
    public async Task<ActionResult<List<InventoryValuationReportDto>>> GetInventoryValuation(
        [FromQuery] List<Guid>? categoryIds = null,
        [FromQuery] List<Guid>? productIds = null,
        [FromQuery] bool includeRetired = false,
        [FromQuery] DateTime? asOfDate = null)
    {
        var query = new GetInventoryValuationQuery(categoryIds, productIds, includeRetired, asOfDate);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get maintenance alerts for items requiring attention
    /// </summary>
    /// <param name="urgent">Only show urgent alerts (default: false)</param>
    /// <returns>Items requiring maintenance attention</returns>
    [HttpGet("maintenance-alerts")]
    public async Task<ActionResult<object>> GetMaintenanceAlerts([FromQuery] bool urgent = false)
    {
        var agingQuery = new GetInventoryAgingQuery(MaintenanceThresholdDays: urgent ? 90 : 180);
        var agingReport = await mediator.Send(agingQuery);
        
        var alerts = agingReport
            .Where(item => item.NeedsMaintenance || (urgent && item.DaysSinceLastMaintenance > 90))
            .Select(item => new
            {
                item.InventoryItemId,
                item.ProductId,
                item.ProductName,
                item.Size,
                item.Color,
                item.SerialNumber,
                item.Condition,
                item.DaysSinceLastMaintenance,
                item.DaysSinceLastRental,
                AlertLevel = item.DaysSinceLastMaintenance > 270 ? "Critical" : 
                           item.DaysSinceLastMaintenance > 180 ? "High" : "Medium",
                RecommendedAction = item.DaysSinceLastMaintenance > 270 ? "Immediate maintenance required" :
                                  item.DaysSinceLastMaintenance > 180 ? "Schedule maintenance soon" :
                                  "Consider maintenance scheduling"
            })
            .OrderByDescending(a => a.DaysSinceLastMaintenance)
            .ToList();

        return Ok(new
        {
            AlertDate = DateTime.UtcNow,
            TotalAlerts = alerts.Count,
            CriticalAlerts = alerts.Count(a => a.AlertLevel == "Critical"),
            HighPriorityAlerts = alerts.Count(a => a.AlertLevel == "High"),
            MediumPriorityAlerts = alerts.Count(a => a.AlertLevel == "Medium"),
            Alerts = alerts
        });
    }

    /// <summary>
    /// Get utilization summary showing how efficiently inventory is being used
    /// </summary>
    /// <param name="categoryIds">Filter by category IDs (optional)</param>
    /// <returns>Utilization metrics by product and category</returns>
    [HttpGet("utilization")]
    public async Task<ActionResult<object>> GetUtilizationReport([FromQuery] List<Guid>? categoryIds = null)
    {
        var performanceQuery = new GetProductPerformanceReportQuery(categoryIds);
        var performanceReport = await mediator.Send(performanceQuery);
        
        var utilizationData = performanceReport
            .Select(p => new
            {
                p.ProductId,
                p.ProductName,
                p.CategoryName,
                p.TotalInventoryItems,
                p.AvailableItems,
                p.RentedItems,
                p.UtilizationRate,
                p.TurnoverRate,
                p.TotalRentals,
                EfficiencyScore = CalculateEfficiencyScore(p.UtilizationRate, p.TurnoverRate),
                Recommendation = GetUtilizationRecommendation(p.UtilizationRate, p.TurnoverRate)
            })
            .OrderByDescending(u => u.EfficiencyScore)
            .ToList();

        return Ok(new
        {
            ReportDate = DateTime.UtcNow,
            OverallUtilization = utilizationData.Where(u => u.TotalInventoryItems > 0).Average(u => u.UtilizationRate),
            HighPerformers = utilizationData.Where(u => u.EfficiencyScore > 75).Count(),
            LowPerformers = utilizationData.Where(u => u.EfficiencyScore < 25).Count(),
            Products = utilizationData
        });
    }

    private static double CalculateEfficiencyScore(double utilizationRate, double turnoverRate)
    {
        // Weighted score: 60% utilization, 40% turnover
        return (utilizationRate * 0.6) + (Math.Min(turnoverRate * 10, 100) * 0.4);
    }

    private static string GetUtilizationRecommendation(double utilizationRate, double turnoverRate)
    {
        return (utilizationRate, turnoverRate) switch
        {
            ( > 80, > 8) => "Excellent performance - consider expanding inventory",
            ( > 60, > 5) => "Good performance - monitor trends",
            ( > 40, > 3) => "Moderate performance - optimize marketing",
            ( > 20, _) => "Low utilization - review pricing or reduce inventory",
            _ => "Very low utilization - consider retirement or repricing"
        };
    }
    
    /// <summary>
    /// Get trend analysis showing rental patterns over time
    /// </summary>
    /// <param name="categoryIds">Filter by category IDs (optional)</param>
    /// <param name="fromDate">Start date for analysis (optional)</param>
    /// <param name="toDate">End date for analysis (optional)</param>
    /// <param name="groupBy">Group by period: day, week, month, quarter (default: month)</param>
    /// <returns>Trend analysis data over specified period</returns>
    [HttpGet("trends")]
    public async Task<ActionResult<List<TrendAnalysisReportDto>>> GetTrendAnalysis(
        [FromQuery] List<Guid>? categoryIds = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string groupBy = "month")
    {
        var query = new GetTrendAnalysisQuery(categoryIds, fromDate, toDate, groupBy);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get demand analysis for sizes, colors, and categories
    /// </summary>
    /// <param name="categoryIds">Filter by category IDs (optional)</param>
    /// <param name="fromDate">Start date for analysis (optional)</param>
    /// <param name="toDate">End date for analysis (optional)</param>
    /// <param name="analysisType">Type of analysis: size, color, category, all (default: all)</param>
    /// <returns>Demand analysis data by specified dimensions</returns>
    [HttpGet("demand")]
    public async Task<ActionResult<List<DemandAnalysisReportDto>>> GetDemandAnalysis(
        [FromQuery] List<Guid>? categoryIds = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string analysisType = "all")
    {
        var query = new GetDemandAnalysisQuery(categoryIds, fromDate, toDate, analysisType);
        var result = await mediator.Send(query);
        return Ok(result);
    }
}
