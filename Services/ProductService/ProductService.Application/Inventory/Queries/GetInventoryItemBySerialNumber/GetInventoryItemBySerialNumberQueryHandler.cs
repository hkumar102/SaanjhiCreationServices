using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.Persistence;
using Shared.ErrorHandling;

namespace ProductService.Application.Inventory.Queries.GetInventoryItemBySerialNumber;

public class GetInventoryItemBySerialNumberQueryHandler : IRequestHandler<GetInventoryItemBySerialNumberQuery, InventoryItemDto?>
{
    private readonly ProductDbContext _db;
    private readonly IMapper _mapper;
    private readonly ILogger<GetInventoryItemBySerialNumberQueryHandler> _logger;

    public GetInventoryItemBySerialNumberQueryHandler(ProductDbContext db, IMapper mapper, ILogger<GetInventoryItemBySerialNumberQueryHandler> logger)
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<InventoryItemDto?> Handle(GetInventoryItemBySerialNumberQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Handling GetInventoryItemBySerialNumberQuery for SerialNumber: {SerialNumber}", request.SerialNumber);
        var entity = await _db.InventoryItems.Include(i => i.Product).FirstOrDefaultAsync(i => i.SerialNumber == request.SerialNumber, cancellationToken);
        if (entity == null)
        {
            _logger.LogDebug("No inventory item found for SerialNumber: {SerialNumber}", request.SerialNumber);
            throw new BusinessRuleException($"Inventory item with SerialNumber '{request.SerialNumber}' not found.");
        }
        _logger.LogDebug("Inventory item found for SerialNumber: {SerialNumber}, Id: {InventoryItemId}", request.SerialNumber, entity.Id);
        return _mapper.Map<InventoryItemDto>(entity);
    }
}
