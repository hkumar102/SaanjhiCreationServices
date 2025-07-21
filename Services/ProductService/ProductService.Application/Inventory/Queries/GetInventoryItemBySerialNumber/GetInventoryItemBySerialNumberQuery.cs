using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Inventory.Queries.GetInventoryItemBySerialNumber;

public record GetInventoryItemBySerialNumberQuery(string SerialNumber) : IRequest<InventoryItemDto?>;
