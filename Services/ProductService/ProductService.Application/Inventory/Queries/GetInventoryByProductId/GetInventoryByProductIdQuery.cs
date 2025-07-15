using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Inventory.Queries.GetInventoryByProductId;

public record GetInventoryByProductIdQuery(
    Guid ProductId,
    string? Size = null,
    string? Color = null,
    Contracts.Enums.InventoryStatus? Status = null,
    Contracts.Enums.ItemCondition? Condition = null,
    bool IncludeRetired = false
) : IRequest<List<InventoryItemDto>>;
