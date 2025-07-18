using MediatR;
using ProductService.Contracts.DTOs;
using ProductService.Contracts.Enums;
using Shared.Contracts.Common;

namespace ProductService.Application.Inventory.Queries.SearchInventory;

public record SearchInventoryQuery(
    int Page = 1,
    int PageSize = 20,
    List<string>? Sizes = null,
    List<string>? Colors = null,
    InventoryStatus? Status = null,
    ItemCondition? Condition = null,
    bool IncludeRetired = false,
    string? SortBy = null,
    bool SortDesc = false,
    Guid? ProductId = null,
    List<Guid>? ProductIds = null,
    decimal? AcquisitionCostMin = null,
    decimal? AcquisitionCostMax = null
) : IRequest<PaginatedResult<InventoryItemDto>>;
