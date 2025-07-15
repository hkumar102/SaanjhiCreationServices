using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Reports.Queries.GetInventoryAging;

public record GetInventoryAgingQuery(
    List<Guid>? CategoryIds = null,
    List<string>? Sizes = null,
    List<string>? Colors = null,
    int MaintenanceThresholdDays = 180,
    int LowActivityThresholdDays = 90,
    bool IncludeRetired = false
) : IRequest<List<InventoryAgingReportDto>>;
