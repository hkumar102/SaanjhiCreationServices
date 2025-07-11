using MediatR;
using ProductService.Contracts.DTOs;
using Shared.Contracts.Common;

namespace ProductService.Application.Products.Queries.GetAllProducts;

public class GetAllProductsQuery : IRequest<PaginatedResult<ProductDto>>
{
    // Basic Search & Filtering
    public string? Search { get; set; }
    public List<Guid>? CategoryIds { get; set; }
    public bool? IsRentable { get; set; }
    public bool? IsPurchasable { get; set; }
    public bool? IsActive { get; set; }

    // Pricing Filters
    public decimal? MinPurchasePrice { get; set; }
    public decimal? MaxPurchasePrice { get; set; }
    public decimal? MinRentalPrice { get; set; }
    public decimal? MaxRentalPrice { get; set; }

    // Product Specifications (New)
    public string? Brand { get; set; }
    public string? Designer { get; set; }
    public List<string>? Sizes { get; set; }
    public List<string>? Colors { get; set; }
    public string? Material { get; set; }
    public string? Occasion { get; set; }
    public string? Season { get; set; }

    // Inventory Filters (New)
    public bool? HasAvailableInventory { get; set; }
    public int? MinAvailableQuantity { get; set; }

    // Sorting
    public string? SortBy { get; set; }
    public bool SortDesc { get; set; } = false;

    // Pagination
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    // Include Options
    public bool IncludeMedia { get; set; } = true;
    public bool IncludeInventory { get; set; } = false;
    public bool OrganizeMediaByColor { get; set; } = true;
}
