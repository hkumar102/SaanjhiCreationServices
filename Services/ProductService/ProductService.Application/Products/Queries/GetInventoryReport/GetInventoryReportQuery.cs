using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Products.Queries.GetInventoryReport;

public record GetInventoryReportQuery(
    List<Guid>? CategoryIds = null,
    List<string>? Sizes = null,
    List<string>? Colors = null,
    Contracts.Enums.InventoryStatus? Status = null,
    bool IncludeRetired = false
) : IRequest<List<InventoryReportDto>>;
