using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.Persistence;
using Shared.Contracts.Common;

namespace ProductService.Application.Inventory.Queries.SearchInventory;

public class SearchInventoryQueryHandler(
    ProductDbContext db,
    IMapper mapper,
    ILogger<SearchInventoryQueryHandler> logger)
    : IRequestHandler<SearchInventoryQuery, PaginatedResult<InventoryItemDto>>
{
    public async Task<PaginatedResult<InventoryItemDto>> Handle(SearchInventoryQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting SearchInventoryQuery: Page={Page}, PageSize={PageSize}", request.Page, request.PageSize);

        var query = db.InventoryItems.Include(i => i.Product).AsQueryable();

        if(!string.IsNullOrWhiteSpace(request.SerialNumber))
        {
            logger.LogDebug("Filtering by SerialNumber: {SerialNumber}", request.SerialNumber);
            query = query.Where(i => EF.Functions.ILike(i.SerialNumber, $"%{request.SerialNumber}%"));
        }
        // Filtering
        if (request.Sizes != null && request.Sizes.Any())
            query = query.Where(i => request.Sizes.Contains(i.Size));
        if (request.Colors != null && request.Colors.Any())
            query = query.Where(i => request.Colors.Contains(i.Color));
        if (request.Statuses != null && request.Statuses.Any())
            query = query.Where(i => request.Statuses.Contains(i.Status));
        if (request.Conditions != null && request.Conditions.Any())
            query = query.Where(i => request.Conditions.Contains(i.Condition));
        if (!request.IncludeRetired)
            query = query.Where(i => !i.IsRetired);
        if (request.ProductId.HasValue)
            query = query.Where(i => i.ProductId == request.ProductId);
        if (request.ProductIds != null && request.ProductIds.Any())
            query = query.Where(i => request.ProductIds.Contains(i.ProductId));

        if (request.AcquisitionCostMin.HasValue)
            query = query.Where(i => i.AcquisitionCost >= request.AcquisitionCostMin.Value);
        if (request.AcquisitionCostMax.HasValue)
            query = query.Where(i => i.AcquisitionCost <= request.AcquisitionCostMax.Value);

        // Sorting
        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            query = request.SortBy.ToLower() switch
            {
                "size" => request.SortDesc ? query.OrderByDescending(i => i.Size) : query.OrderBy(i => i.Size),
                "color" => request.SortDesc ? query.OrderByDescending(i => i.Color) : query.OrderBy(i => i.Color),
                "acquisitiondate" => request.SortDesc ? query.OrderByDescending(i => i.AcquisitionDate) : query.OrderBy(i => i.AcquisitionDate),
                "status" => request.SortDesc ? query.OrderByDescending(i => i.Status) : query.OrderBy(i => i.Status),
                _ => query.OrderByDescending(i => i.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(i => i.CreatedAt);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(i => mapper.Map<InventoryItemDto>(i))
            .ToListAsync(cancellationToken);

        return new PaginatedResult<InventoryItemDto>
        {
            Items = items,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
