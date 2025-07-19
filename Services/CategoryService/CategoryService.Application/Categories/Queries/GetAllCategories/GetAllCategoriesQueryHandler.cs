using Shared.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CategoryService.Infrastructure.Persistence;
using CategoryService.Contracts.DTOs;
using Shared.Contracts.Common;

namespace CategoryService.Application.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler(
    CategoryDbContext db, 
    IMapper mapper,
    ILogger<GetAllCategoriesQueryHandler> logger)
    : IRequestHandler<GetAllCategoriesQuery, PaginatedResult<CategoryDto>>
{
    public async Task<PaginatedResult<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetAllCategoriesQuery execution with Page={Page}, PageSize={PageSize}, Search={Search}",
            request.Page, request.PageSize, request.Search);

        try
        {
            var query = db.Categories.AsNoTracking().AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                logger.LogDebug("Filtering by search term: {Search}", request.Search);
                query = query
                    .WhereContainsIgnoreCase(c => c.Name, request.Search)
                    .WhereContainsIgnoreCase(c => c.Description, request.Search);
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                logger.LogDebug("Applying sorting: {SortBy}, Descending={SortDesc}", request.SortBy, request.SortDesc);
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
            logger.LogDebug("Counting total records for pagination");
            var totalCount = await query.CountAsync(cancellationToken);
            logger.LogDebug("Found {TotalCount} total records", totalCount);

            // Apply pagination
            logger.LogDebug("Applying pagination: Skip={Skip}, Take={Take}", 
                (request.Page - 1) * request.PageSize, request.PageSize);
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ProjectTo<CategoryDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            logger.LogDebug("Retrieved {CategoryCount} categories for current page", items.Count);

            var paginatedResult = new PaginatedResult<CategoryDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };

            logger.LogDebug("GetAllCategoriesQuery completed successfully. Returning {ItemCount} items, Page {Page}, Total={TotalCount}", 
                paginatedResult.Items.Count, paginatedResult.PageNumber, paginatedResult.TotalCount);

            return paginatedResult;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing GetAllCategoriesQuery with Page={Page}, PageSize={PageSize}", 
                request.Page, request.PageSize);
            throw;
        }
    }
}
