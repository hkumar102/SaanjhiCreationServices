using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Inventory.Queries.GetInventoryItemById;

public record GetInventoryItemByIdQuery(Guid InventoryItemId) : IRequest<InventoryItemDto>;
