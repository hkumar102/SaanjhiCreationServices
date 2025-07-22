using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.Persistence;
using Shared.ErrorHandling;

namespace ProductService.Application.Inventory.Queries.GetInventoryItemById;

public class GetInventoryItemByIdQueryHandler : IRequestHandler<GetInventoryItemByIdQuery, InventoryItemDto>
{
    private readonly ProductDbContext _db;
    private readonly IMapper _mapper;

    public GetInventoryItemByIdQueryHandler(ProductDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<InventoryItemDto> Handle(GetInventoryItemByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.InventoryItems
            .Include(i => i.Product) // Include Product to get product details
            .ThenInclude(p => p.Media) // Include Product Media
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.InventoryItemId, cancellationToken);

        if (entity == null)
            throw new BusinessRuleException($"Inventory item with ID {request.InventoryItemId} not found");

        return _mapper.Map<InventoryItemDto>(entity);
    }
}
