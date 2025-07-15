using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Reports.Queries.GetInventoryReport;

public record GetInventoryReportQuery(
    List<Guid>? CategoryIds = null,
    List<string>? Sizes = null,
    List<string>? Colors = null,
    Contracts.Enums.InventoryStatus? Status = null,
    Contracts.Enums.ItemCondition? Condition = null,
    bool IncludeRetired = false,
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IRequest<List<InventoryReportDto>>;
