using MediatR;
using ProductService.Contracts.DTOs;
using Shared.Contracts.Common;

namespace ProductService.Application.Products.Queries.SearchProducts;

public class SearchProductsQuery : IRequest<PaginatedResult<ProductDto>>
{
    public string? SearchTerm { get; set; }
    public List<Guid>? CategoryIds { get; set; }
    public List<string>? Sizes { get; set; }
    public List<string>? Colors { get; set; }
    public string? Brand { get; set; }
    public string? Designer { get; set; }
    public string? Material { get; set; }
    public string? Occasion { get; set; }
    public string? Season { get; set; }
    
    public decimal? MinRentalPrice { get; set; }
    public decimal? MaxRentalPrice { get; set; }
    public decimal? MinPurchasePrice { get; set; }
    public decimal? MaxPurchasePrice { get; set; }
    
    public bool? IsRentable { get; set; }
    public bool? IsPurchasable { get; set; }
    public bool? IsActive { get; set; }
    public bool? HasAvailableInventory { get; set; }
    
    public string? SortBy { get; set; } = "relevance";
    public bool SortDesc { get; set; } = false;
    
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
