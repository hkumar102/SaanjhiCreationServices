using MediatR;
using ProductService.Contracts.DTOs;
using Shared.Contracts.Common;

namespace ProductService.Application.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQuery : IRequest<PaginatedResult<CategoryDto>>
{
    public string? Search { get; set; }
    
    public string? SortBy { get; set; }
    public bool SortDesc { get; set; } = false;
    
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
