using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.HttpClients;
using ProductService.Infrastructure.Persistence;
using Shared.Contracts.Common;

namespace ProductService.Application.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler(
    ProductDbContext db, 
    IMapper mapper, 
    CategoryApiClient categoryApiClient,
    ILogger<GetAllProductsQueryHandler> logger)
    : IRequestHandler<GetAllProductsQuery, PaginatedResult<ProductDto>>
{
    public async Task<PaginatedResult<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetAllProductsQuery execution with Page={Page}, PageSize={PageSize}, Search={Search}, CategoryIds={CategoryIds}",
            request.Page, request.PageSize, request.Search, request.CategoryIds?.Count ?? 0);

        try
        {
            var query = db.Products
                .Include(p => p.Media)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                logger.LogDebug("Filtering by search term: {Search}", request.Search);
                query = query.Where(p =>
                    p.Name.Contains(request.Search) ||
                    (p.Description != null && p.Description.Contains(request.Search)));
            }

            if (request.CategoryIds != null && request.CategoryIds.Any())
            {
                logger.LogDebug("Filtering by {CategoryCount} category IDs", request.CategoryIds.Count);
                query = query.Where(p => request.CategoryIds.Contains(p.CategoryId));
            }

            if (request.IsRentable.HasValue)
            {
                logger.LogDebug("Filtering by IsRentable: {IsRentable}", request.IsRentable);
                query = query.Where(p => p.IsRentable == request.IsRentable);
            }

            if (request.IsActive.HasValue)
            {
                logger.LogDebug("Filtering by IsActive: {IsActive}", request.IsActive);
                query = query.Where(p => p.IsActive == request.IsActive);
            }

            if (request.MinPrice.HasValue)
            {
                logger.LogDebug("Filtering by MinPrice: {MinPrice}", request.MinPrice);
                query = query.Where(p => p.Price >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                logger.LogDebug("Filtering by MaxPrice: {MaxPrice}", request.MaxPrice);
                query = query.Where(p => p.Price <= request.MaxPrice.Value);
            }

            if (request.MinRentalPrice.HasValue)
            {
                logger.LogDebug("Filtering by MinRentalPrice: {MinRentalPrice}", request.MinRentalPrice);
                query = query.Where(p => p.RentalPrice >= request.MinRentalPrice.Value);
            }

            if (request.MaxRentalPrice.HasValue)
            {
                logger.LogDebug("Filtering by MaxRentalPrice: {MaxRentalPrice}", request.MaxRentalPrice);
                query = query.Where(p => p.RentalPrice <= request.MaxRentalPrice.Value);
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                logger.LogDebug("Applying sorting: {SortBy}, Descending={SortDesc}", request.SortBy, request.SortDesc);
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

            logger.LogDebug("Counting total records for pagination");
            var totalCount = await query.CountAsync(cancellationToken);
            logger.LogDebug("Found {TotalCount} total records", totalCount);

            logger.LogDebug("Applying pagination: Skip={Skip}, Take={Take}", 
                (request.Page - 1) * request.PageSize, request.PageSize);
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ProjectTo<ProductDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
            
            logger.LogDebug("Retrieved {ProductCount} products for current page", items.Count);
            
            // Fetch categories in bulk
            var categoryIds = items.Select(p => p.CategoryId).Distinct().ToList();
            logger.LogDebug("Fetching {CategoryCount} categories from CategoryService", categoryIds.Count);
            
            var categories = await categoryApiClient.GetCategoryByIdsAsync(categoryIds) ?? [];
            logger.LogDebug("Received {CategoryCount} categories from CategoryService", categories.Count);

            // Map category names to products
            var categoryDict = categories.ToDictionary(c => c.Id, c => c.Name);
            foreach (var dto in items)
            {
                if (dto.CategoryId != Guid.Empty && categoryDict.TryGetValue(dto.CategoryId, out var name))
                    dto.CategoryName = name;
            }

            var paginatedResult = new PaginatedResult<ProductDto>
            {
                Items = items,
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };

            logger.LogDebug("GetAllProductsQuery completed successfully. Returning {ItemCount} items, Page {Page}, Total={TotalCount}", 
                paginatedResult.Items.Count, paginatedResult.PageNumber, paginatedResult.TotalCount);

            return paginatedResult;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing GetAllProductsQuery with Page={Page}, PageSize={PageSize}", 
                request.Page, request.PageSize);
            throw;
        }
    }
}
