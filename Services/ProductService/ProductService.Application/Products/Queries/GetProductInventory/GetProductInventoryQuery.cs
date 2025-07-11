using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Products.Queries.GetProductInventory;

public record GetProductInventoryQuery(
    Guid ProductId,
    string? Size = null,
    string? Color = null,
    Contracts.Enums.InventoryStatus? Status = null,
    bool IncludeRetired = false
) : IRequest<List<InventoryItemDto>>;
