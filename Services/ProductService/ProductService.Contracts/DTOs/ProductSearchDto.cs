using ProductService.Contracts.Enums;

namespace ProductService.Contracts.DTOs;

/// <summary>
/// DTO for product search and filtering
/// </summary>
public class ProductSearchDto
{
    public string? SearchTerm { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Brand { get; set; }
    public string? Designer { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public string? Material { get; set; }
    public string? Occasion { get; set; }
    public string? Season { get; set; }
    
    // Price Range
    public decimal? MinRentalPrice { get; set; }
    public decimal? MaxRentalPrice { get; set; }
    public decimal? MinPurchasePrice { get; set; }
    public decimal? MaxPurchasePrice { get; set; }
    
    // Availability
    public bool? IsRentable { get; set; }
    public bool? IsPurchasable { get; set; }
    public bool? IsActive { get; set; }
    public bool? HasAvailableInventory { get; set; }
    
    // Sorting
    public string? SortBy { get; set; } // "name", "rentalPrice", "purchasePrice", "createdAt"
    public bool SortDescending { get; set; } = false;
    
    // Pagination
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// DTO for paginated product search results
/// </summary>
public class ProductSearchResultDto
{
    public List<ProductDto> Products { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}

/// <summary>
/// DTO for inventory availability search
/// </summary>
public class InventoryAvailabilityDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string? Size { get; set; }
    public string? Color { get; set; }
    public int AvailableCount { get; set; }
    public List<InventoryItemDto> AvailableItems { get; set; } = new();
}
