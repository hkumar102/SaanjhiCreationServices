using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Inventory.Commands.UpdateInventoryItemStatus;

public record UpdateInventoryItemStatusCommand(
    Guid InventoryItemId,
    Contracts.Enums.InventoryStatus Status,
    string? Notes = null
) : IRequest<InventoryItemDto>;
