using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Products.Commands.UpdateInventoryStatus;

public record UpdateInventoryStatusCommand(
    Guid InventoryItemId,
    Contracts.Enums.InventoryStatus Status,
    string? ConditionNotes = null
) : IRequest<InventoryItemDto>;
