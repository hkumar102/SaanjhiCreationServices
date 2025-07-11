using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Infrastructure.Persistence;
using Shared.Contracts.Common;

namespace ProductService.Application.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler(
    ProductDbContext db, 
    IMapper mapper, 
    ILogger<GetAllProductsQueryHandler> logger)
    : IRequestHandler<GetAllProductsQuery, PaginatedResult<ProductDto>>
{
    public async Task<PaginatedResult<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetAllProductsQuery execution with Page={Page}, PageSize={PageSize}, Search={Search}, CategoryIds={CategoryIds}",
            request.Page, request.PageSize, request.Search, request.CategoryIds?.Count ?? 0);

        try
        {
            // Build base query with includes
            var query = db.Products.AsQueryable();

            // Include media if requested
            if (request.IncludeMedia)
            {
                query = query.Include(p => p.Media);
            }

            // Include inventory if requested
            if (request.IncludeInventory)
            {
                query = query.Include(p => p.InventoryItems);
            }

            // Apply filters
            query = ApplyBasicFilters(query, request);
            query = ApplyPricingFilters(query, request);
            query = ApplySpecificationFilters(query, request);
            query = ApplyInventoryFilters(query, request);

            // Apply sorting
            query = ApplySorting(query, request);

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
            
            // Enhance DTOs with additional data
            await EnhanceProductDtos(items, request, cancellationToken);

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

    private IQueryable<Domain.Entities.Product> ApplyBasicFilters(IQueryable<Domain.Entities.Product> query, GetAllProductsQuery request)
    {
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            logger.LogDebug("Filtering by search term: {Search}", request.Search);
            query = query.Where(p =>
                p.Name.Contains(request.Search) ||
                (p.Description != null && p.Description.Contains(request.Search)) ||
                (p.Brand != null && p.Brand.Contains(request.Search)) ||
                (p.Designer != null && p.Designer.Contains(request.Search)));
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

        if (request.IsPurchasable.HasValue)
        {
            logger.LogDebug("Filtering by IsPurchasable: {IsPurchasable}", request.IsPurchasable);
            query = query.Where(p => p.IsPurchasable == request.IsPurchasable);
        }

        if (request.IsActive.HasValue)
        {
            logger.LogDebug("Filtering by IsActive: {IsActive}", request.IsActive);
            query = query.Where(p => p.IsActive == request.IsActive);
        }

        return query;
    }

    private IQueryable<Domain.Entities.Product> ApplyPricingFilters(IQueryable<Domain.Entities.Product> query, GetAllProductsQuery request)
    {
        if (request.MinPurchasePrice.HasValue)
        {
            logger.LogDebug("Filtering by MinPurchasePrice: {MinPurchasePrice}", request.MinPurchasePrice);
            query = query.Where(p => p.PurchasePrice >= request.MinPurchasePrice.Value);
        }

        if (request.MaxPurchasePrice.HasValue)
        {
            logger.LogDebug("Filtering by MaxPurchasePrice: {MaxPurchasePrice}", request.MaxPurchasePrice);
            query = query.Where(p => p.PurchasePrice <= request.MaxPurchasePrice.Value);
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

        return query;
    }

    private IQueryable<Domain.Entities.Product> ApplySpecificationFilters(IQueryable<Domain.Entities.Product> query, GetAllProductsQuery request)
    {
        if (!string.IsNullOrWhiteSpace(request.Brand))
        {
            logger.LogDebug("Filtering by Brand: {Brand}", request.Brand);
            query = query.Where(p => p.Brand != null && p.Brand.Contains(request.Brand));
        }

        if (!string.IsNullOrWhiteSpace(request.Designer))
        {
            logger.LogDebug("Filtering by Designer: {Designer}", request.Designer);
            query = query.Where(p => p.Designer != null && p.Designer.Contains(request.Designer));
        }

        if (!string.IsNullOrWhiteSpace(request.Material))
        {
            logger.LogDebug("Filtering by Material: {Material}", request.Material);
            query = query.Where(p => p.Material != null && p.Material.Contains(request.Material));
        }

        if (!string.IsNullOrWhiteSpace(request.Occasion))
        {
            logger.LogDebug("Filtering by Occasion: {Occasion}", request.Occasion);
            query = query.Where(p => p.Occasion != null && p.Occasion.Contains(request.Occasion));
        }

        if (!string.IsNullOrWhiteSpace(request.Season))
        {
            logger.LogDebug("Filtering by Season: {Season}", request.Season);
            query = query.Where(p => p.Season != null && p.Season.Contains(request.Season));
        }

        if (request.Sizes != null && request.Sizes.Any())
        {
            logger.LogDebug("Filtering by Sizes: {Sizes}", string.Join(", ", request.Sizes));
            foreach (var size in request.Sizes)
            {
                query = query.Where(p => p.AvailableSizes.Contains(size));
            }
        }

        if (request.Colors != null && request.Colors.Any())
        {
            logger.LogDebug("Filtering by Colors: {Colors}", string.Join(", ", request.Colors));
            foreach (var color in request.Colors)
            {
                query = query.Where(p => p.AvailableColors.Contains(color));
            }
        }

        return query;
    }

    private IQueryable<Domain.Entities.Product> ApplyInventoryFilters(IQueryable<Domain.Entities.Product> query, GetAllProductsQuery request)
    {
        if (request.HasAvailableInventory.HasValue && request.HasAvailableInventory.Value)
        {
            logger.LogDebug("Filtering by HasAvailableInventory: {HasAvailableInventory}", request.HasAvailableInventory);
            query = query.Where(p => p.InventoryItems.Any(i => 
                i.Status == Contracts.Enums.InventoryStatus.Available && !i.IsRetired));
        }

        if (request.MinAvailableQuantity.HasValue)
        {
            logger.LogDebug("Filtering by MinAvailableQuantity: {MinAvailableQuantity}", request.MinAvailableQuantity);
            query = query.Where(p => p.InventoryItems.Count(i => 
                i.Status == Contracts.Enums.InventoryStatus.Available && !i.IsRetired) >= request.MinAvailableQuantity.Value);
        }

        return query;
    }

    private IQueryable<Domain.Entities.Product> ApplySorting(IQueryable<Domain.Entities.Product> query, GetAllProductsQuery request)
    {
        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            logger.LogDebug("Applying sorting: {SortBy}, Descending={SortDesc}", request.SortBy, request.SortDesc);
            query = request.SortBy.ToLower() switch
            {
                "name" => request.SortDesc ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "purchaseprice" => request.SortDesc ? query.OrderByDescending(p => p.PurchasePrice) : query.OrderBy(p => p.PurchasePrice),
                "rentalprice" => request.SortDesc ? query.OrderByDescending(p => p.RentalPrice) : query.OrderBy(p => p.RentalPrice),
                "brand" => request.SortDesc ? query.OrderByDescending(p => p.Brand) : query.OrderBy(p => p.Brand),
                "designer" => request.SortDesc ? query.OrderByDescending(p => p.Designer) : query.OrderBy(p => p.Designer),
                "createdat" => request.SortDesc ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                "modifiedat" => request.SortDesc ? query.OrderByDescending(p => p.ModifiedAt) : query.OrderBy(p => p.ModifiedAt),
                _ => query.OrderByDescending(p => p.CreatedAt) // default fallback
            };
        }
        else
        {
            query = query.OrderByDescending(p => p.CreatedAt); // default when no sort specified
        }

        return query;
    }

    private async Task EnhanceProductDtos(List<ProductDto> items, GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        // Fetch categories in bulk
        await FetchCategoryNames(items, cancellationToken);

        // Calculate inventory counts
        CalculateInventoryCounts(items);

        // Organize media by color if requested
        if (request.OrganizeMediaByColor && request.IncludeMedia)
        {
            OrganizeMediaByColor(items);
        }
    }

    private async Task FetchCategoryNames(List<ProductDto> items, CancellationToken cancellationToken)
    {
        var categoryIds = items.Select(p => p.CategoryId).Distinct().ToList();
        logger.LogDebug("Fetching {CategoryCount} categories from local database", categoryIds.Count);
        
        var categories = await db.Categories
            .Where(c => categoryIds.Contains(c.Id))
            .Select(c => new { c.Id, c.Name })
            .ToListAsync(cancellationToken);
            
        logger.LogDebug("Received {CategoryCount} categories from local database", categories.Count);

        // Map category names to products
        var categoryDict = categories.ToDictionary(c => c.Id, c => c.Name);
        foreach (var dto in items)
        {
            if (dto.CategoryId != Guid.Empty && categoryDict.TryGetValue(dto.CategoryId, out var name))
                dto.CategoryName = name;
        }
    }

    private void CalculateInventoryCounts(List<ProductDto> items)
    {
        foreach (var item in items)
        {
            item.TotalInventoryCount = item.InventoryItems?.Count ?? 0;
            item.AvailableInventoryCount = item.InventoryItems?.Count(i => 
                i.Status == Contracts.Enums.InventoryStatus.Available && !i.IsRetired) ?? 0;
        }
    }

    private void OrganizeMediaByColor(List<ProductDto> items)
    {
        foreach (var item in items)
        {
            if (item.Media?.Any() == true)
            {
                // Organize media by color
                item.MediaByColor = item.Media
                    .Where(m => !string.IsNullOrEmpty(m.Color) && !m.IsGeneric)
                    .GroupBy(m => m.Color!)
                    .ToDictionary(g => g.Key, g => g.OrderBy(m => m.SortOrder).ToList());

                // Separate generic media
                item.GenericMedia = item.Media
                    .Where(m => m.IsGeneric)
                    .OrderBy(m => m.SortOrder)
                    .ToList();
            }
        }
    }
}
