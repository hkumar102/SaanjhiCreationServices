using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Inventory.Commands.CreateInventoryItem;

public record CreateInventoryItemCommand(
    Guid ProductId,
    string Size,
    string Color,
    Contracts.Enums.ItemCondition Condition,
    decimal AcquisitionCost,
    DateTime? AcquisitionDate = null,
    string? ConditionNotes = null,
    string? WarehouseLocation = null,
    bool IsRetired = false,
    DateTime? RetiredDate = null,
    string? RetirementReason = null
) : IRequest<InventoryItemDto>;
