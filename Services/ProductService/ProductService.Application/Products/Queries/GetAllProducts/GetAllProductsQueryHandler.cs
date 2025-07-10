using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.HttpClients;
using ProductService.Infrastructure.Persistence;
using Shared.Contracts.Common;

namespace ProductService.Application.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler(ProductDbContext db, IMapper mapper, CategoryApiClient categoryApiClient)
    : IRequestHandler<GetAllProductsQuery, PaginatedResult<ProductDto>>
{
    public async Task<PaginatedResult<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var query = db.Products
            .Include(p => p.Media)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(p =>
                p.Name.Contains(request.Search) ||
                (p.Description != null && p.Description.Contains(request.Search)));
        }

        if (request.CategoryIds != null && request.CategoryIds.Any())
            query = query.Where(p => request.CategoryIds.Contains(p.CategoryId));

        if (request.IsRentable.HasValue)
            query = query.Where(p => p.IsRentable == request.IsRentable);

        if (request.IsActive.HasValue)
            query = query.Where(p => p.IsActive == request.IsActive);

        if (request.MinPrice.HasValue)
            query = query.Where(p => p.Price >= request.MinPrice.Value);

        if (request.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= request.MaxPrice.Value);

        if (request.MinRentalPrice.HasValue)
            query = query.Where(p => p.RentalPrice >= request.MinRentalPrice.Value);

        if (request.MaxRentalPrice.HasValue)
            query = query.Where(p => p.RentalPrice <= request.MaxRentalPrice.Value);

        // Sorting
        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            query = request.SortBy.ToLower() switch
            {
                "name" => request.SortDesc ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "price" => request.SortDesc ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                "rentalprice" => request.SortDesc ? query.OrderByDescending(p => p.RentalPrice) : query.OrderBy(p => p.RentalPrice),
                "quantity" => request.SortDesc ? query.OrderByDescending(p => p.Quantity) : query.OrderBy(p => p.Quantity),
                "createdat" => request.SortDesc ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                "modifiedat" => request.SortDesc ? query.OrderByDescending(p => p.ModifiedAt) : query.OrderBy(p => p.ModifiedAt),
                _ => query.OrderByDescending(p => p.CreatedAt) // default fallback
            };
        }
        else
        {
            query = query.OrderByDescending(p => p.CreatedAt); // default when no sort specified
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<ProductDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        
        // Fetch categories in bulk
        var categoryIds = items.Select(p => p.CategoryId).Distinct().ToList();
        var categories = await categoryApiClient.GetCategoryByIdsAsync(categoryIds) ?? [];

        // Map category names to products
        var categoryDict = categories.ToDictionary(c => c.Id, c => c.Name);
        foreach (var dto in items)
        {
            if (dto.CategoryId != Guid.Empty && categoryDict.TryGetValue(dto.CategoryId, out var name))
                dto.CategoryName = name;
        }
        return new PaginatedResult<ProductDto>
        {
            Items = items,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
