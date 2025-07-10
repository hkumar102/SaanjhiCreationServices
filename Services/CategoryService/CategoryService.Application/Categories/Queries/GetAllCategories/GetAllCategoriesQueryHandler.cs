using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CategoryService.Infrastructure.Persistence;
using CategoryService.Contracts.DTOs;
using Shared.Contracts.Common;

namespace CategoryService.Application.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler(CategoryDbContext db, IMapper mapper)
    : IRequestHandler<GetAllCategoriesQuery, PaginatedResult<CategoryDto>>
{
    public async Task<PaginatedResult<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = db.Categories.AsNoTracking().AsQueryable();

        // Filtering
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(c => 
                c.Name.Contains(request.Search) ||
                (c.Description != null && c.Description.Contains(request.Search)));
        }

        // Sorting
        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            query = request.SortBy.ToLower() switch
            {
                "name" => request.SortDesc ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                "description" => request.SortDesc ? query.OrderByDescending(c => c.Description) : query.OrderBy(c => c.Description),
                "createdat" => request.SortDesc ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
                "modifiedat" => request.SortDesc ? query.OrderByDescending(c => c.ModifiedAt) : query.OrderBy(c => c.ModifiedAt),
                _ => query.OrderBy(c => c.Name) // default fallback
            };
        }
        else
        {
            query = query.OrderBy(c => c.Name); // default when no sort specified
        }

        // Count total records before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<CategoryDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<CategoryDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.Page,
            PageSize = request.PageSize
        };
    }
}
