using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Reports.Queries.GetInventoryValuation;

public record GetInventoryValuationQuery(
    List<Guid>? CategoryIds = null,
    List<Guid>? ProductIds = null,
    bool IncludeRetired = false,
    DateTime? AsOfDate = null
) : IRequest<List<InventoryValuationReportDto>>;
