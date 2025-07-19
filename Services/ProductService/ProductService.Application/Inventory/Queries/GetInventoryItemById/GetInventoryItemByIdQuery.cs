using MediatR;
using ProductService.Contracts.DTOs;
using System;

namespace ProductService.Application.Inventory.Queries.GetInventoryItemById;

public record GetInventoryItemByIdQuery(Guid InventoryItemId) : IRequest<InventoryItemDto>;
