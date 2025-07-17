using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.Persistence;
using Shared.Contracts.Common;

namespace ProductService.Application.Products.Queries.SearchProducts;

public class SearchProductsQueryHandler(
    ProductDbContext db, 
    IMapper mapper, 
    ILogger<SearchProductsQueryHandler> logger)
    : IRequestHandler<SearchProductsQuery, PaginatedResult<ProductDto>>
{
    public async Task<PaginatedResult<ProductDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting SearchProductsQuery execution with SearchTerm={SearchTerm}", request.SearchTerm);

        try
        {
            // Build base query
            var query = db.Products
                .Include(p => p.InventoryItems)
                .Include(p => p.Media)
                .AsQueryable();

            // Apply search filters
            query = ApplySearchFilters(query, request);
            query = ApplySpecificationFilters(query, request);
            query = ApplyPricingFilters(query, request);
            query = ApplyAvailabilityFilters(query, request);

            // Apply sorting
            query = ApplySorting(query, request);

            logger.LogDebug("Counting total records for search pagination");
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
            
            // Enhance DTOs with additional data
            await EnhanceSearchResults(items, cancellationToken);

            var paginatedResult = new PaginatedResult<ProductDto>
            {
                Items = items,
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };

            logger.LogDebug("SearchProductsQuery completed successfully. Returning {ItemCount} items", paginatedResult.Items.Count);

            return paginatedResult;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing SearchProductsQuery with SearchTerm: {SearchTerm}", 
                request.SearchTerm);
            throw;
        }
    }

    private IQueryable<Domain.Entities.Product> ApplySearchFilters(IQueryable<Domain.Entities.Product> query, SearchProductsQuery request)
    {
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            logger.LogDebug("Applying search term: {SearchTerm}", request.SearchTerm);
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(searchTerm) ||
                (p.Description != null && p.Description.ToLower().Contains(searchTerm)) ||
                (p.Brand != null && p.Brand.ToLower().Contains(searchTerm)) ||
                (p.Designer != null && p.Designer.ToLower().Contains(searchTerm)) ||
                (p.Material != null && p.Material.ToLower().Contains(searchTerm)) ||
                (p.Season != null && p.Season.ToLower().Contains(searchTerm)));
        }

        if (request.CategoryIds != null && request.CategoryIds.Any())
        {
            logger.LogDebug("Filtering by {CategoryCount} category IDs", request.CategoryIds.Count);
            query = query.Where(p => request.CategoryIds.Contains(p.CategoryId));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == request.IsActive);
        }

        return query;
    }

    private IQueryable<Domain.Entities.Product> ApplySpecificationFilters(IQueryable<Domain.Entities.Product> query, SearchProductsQuery request)
    {
        if (!string.IsNullOrWhiteSpace(request.Brand))
        {
            query = query.Where(p => p.Brand != null && p.Brand.ToLower().Contains(request.Brand.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(request.Designer))
        {
            query = query.Where(p => p.Designer != null && p.Designer.ToLower().Contains(request.Designer.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(request.Material))
        {
            query = query.Where(p => p.Material != null && p.Material.ToLower().Contains(request.Material.ToLower()));
        }
        

        if (!string.IsNullOrWhiteSpace(request.Season))
        {
            query = query.Where(p => p.Season != null && p.Season.ToLower().Contains(request.Season.ToLower()));
        }
        
        if (request.Occasion != null && request.Occasion.Any())
        {
            foreach (var occasion in request.Occasion)
            {
                query = query.Where(p => p.Occasion.Contains(occasion));
            }
        }

        if (request.Sizes != null && request.Sizes.Any())
        {
            foreach (var size in request.Sizes)
            {
                query = query.Where(p => p.AvailableSizes.Contains(size));
            }
        }

        if (request.Colors != null && request.Colors.Any())
        {
            foreach (var color in request.Colors)
            {
                query = query.Where(p => p.AvailableColors.Contains(color));
            }
        }

        return query;
    }

    private IQueryable<Domain.Entities.Product> ApplyPricingFilters(IQueryable<Domain.Entities.Product> query, SearchProductsQuery request)
    {
        if (request.IsRentable.HasValue)
        {
            query = query.Where(p => p.IsRentable == request.IsRentable);
        }

        if (request.IsPurchasable.HasValue)
        {
            query = query.Where(p => p.IsPurchasable == request.IsPurchasable);
        }

        if (request.MinRentalPrice.HasValue)
        {
            query = query.Where(p => p.RentalPrice >= request.MinRentalPrice.Value);
        }

        if (request.MaxRentalPrice.HasValue)
        {
            query = query.Where(p => p.RentalPrice <= request.MaxRentalPrice.Value);
        }

        if (request.MinPurchasePrice.HasValue)
        {
            query = query.Where(p => p.PurchasePrice >= request.MinPurchasePrice.Value);
        }

        if (request.MaxPurchasePrice.HasValue)
        {
            query = query.Where(p => p.PurchasePrice <= request.MaxPurchasePrice.Value);
        }

        return query;
    }

    private IQueryable<Domain.Entities.Product> ApplyAvailabilityFilters(IQueryable<Domain.Entities.Product> query, SearchProductsQuery request)
    {
        if (request.HasAvailableInventory.HasValue && request.HasAvailableInventory.Value)
        {
            query = query.Where(p => p.InventoryItems.Any(i => 
                i.Status == Contracts.Enums.InventoryStatus.Available && !i.IsRetired));
        }

        return query;
    }

    private IQueryable<Domain.Entities.Product> ApplySorting(IQueryable<Domain.Entities.Product> query, SearchProductsQuery request)
    {
        logger.LogDebug("Applying sorting: {SortBy}, Descending={SortDesc}", request.SortBy, request.SortDesc);
        
        return request.SortBy?.ToLower() switch
        {
            "name" => request.SortDesc ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "rentalprice" => request.SortDesc ? query.OrderByDescending(p => p.RentalPrice) : query.OrderBy(p => p.RentalPrice),
            "purchaseprice" => request.SortDesc ? query.OrderByDescending(p => p.PurchasePrice) : query.OrderBy(p => p.PurchasePrice),
            "brand" => request.SortDesc ? query.OrderByDescending(p => p.Brand) : query.OrderBy(p => p.Brand),
            "createdat" => request.SortDesc ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            "popularity" => query.OrderByDescending(p => p.InventoryItems.Sum(i => i.TimesRented)),
            "availability" => query.OrderByDescending(p => p.InventoryItems.Count(i => 
                i.Status == Contracts.Enums.InventoryStatus.Available && !i.IsRetired)),
            "relevance" or _ => query.OrderByDescending(p => p.CreatedAt) // Default relevance sorting
        };
    }

    private async Task EnhanceSearchResults(List<ProductDto> items, CancellationToken cancellationToken)
    {
        // Fetch categories in bulk
        var categoryIds = items.Select(p => p.CategoryId).Distinct().ToList();
        logger.LogDebug("Fetching {CategoryCount} categories from local database", categoryIds.Count);
        
        var categories = await db.Categories
            .Where(c => categoryIds.Contains(c.Id))
            .Select(c => new { c.Id, c.Name })
            .ToListAsync(cancellationToken);
            
        logger.LogDebug("Received {CategoryCount} categories from local database", categories.Count);

        // Map category names to products
        var categoryDict = categories.ToDictionary(c => c.Id, c => c.Name);
        foreach (var item in items)
        {
            if (item.CategoryId != Guid.Empty && categoryDict.TryGetValue(item.CategoryId, out var name))
                item.CategoryName = name;

            // Note: ProductDto has MainImage property that automatically provides the primary image
            // No need to manually set PrimaryImageUrl since it doesn't exist in DTO
        }
    }
}
