using MediatR;
using ProductService.Contracts.DTOs;
using ProductService.Contracts.Enums;

namespace ProductService.Application.Inventory.Commands.UpdateInventoryItem;

public record UpdateInventoryItemCommand(
    Guid Id,
    string? Size,
    string? Color,
    ItemCondition? Condition,
    decimal? AcquisitionCost,
    DateTime? AcquisitionDate,
    string? ConditionNotes,
    string? WarehouseLocation,
    bool? IsRetired,
    string? RetirementReason,
    DateTime? RetirementDate
) : IRequest<InventoryItemDto>;
