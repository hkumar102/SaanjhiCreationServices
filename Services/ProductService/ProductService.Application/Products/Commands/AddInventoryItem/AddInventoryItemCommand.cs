using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Products.Commands.AddInventoryItem;

public record AddInventoryItemCommand(
    Guid ProductId,
    string Size,
    string Color,
    Contracts.Enums.ItemCondition Condition,
    decimal AcquisitionCost,
    string? SerialNumber = null,
    string? Barcode = null,
    string? ConditionNotes = null,
    string? WarehouseLocation = null
) : IRequest<InventoryItemDto>;
