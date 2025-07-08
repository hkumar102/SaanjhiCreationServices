using MediatR;
using ProductService.Contracts.DTOs;
using Shared.Contracts.Common;

namespace ProductService.Application.Products.Queries.GetAllProducts;

public class GetAllProductsQuery : IRequest<PaginatedResult<ProductDto>>
{
    public string? Search { get; set; }
    public Guid? CategoryId { get; set; }
    public bool? IsRentable { get; set; }
    public bool? IsActive { get; set; }

    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    public decimal? MinRentalPrice { get; set; }
    public decimal? MaxRentalPrice { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
