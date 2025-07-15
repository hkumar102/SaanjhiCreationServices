using MediatR;
using ProductService.Contracts.DTOs;
using Shared.Contracts.Common;

namespace ProductService.Application.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQuery : IRequest<PaginatedResult<CategoryDto>>
{
    /// <summary>
    /// General search term that searches both name and description
    /// </summary>
    public string? Search { get; set; }
    
    /// <summary>
    /// Search specifically in category name
    /// </summary>
    public string? SearchName { get; set; }
    
    /// <summary>
    /// Search specifically in category description
    /// </summary>
    public string? SearchDescription { get; set; }
    
    public string? SortBy { get; set; }
    public bool SortDesc { get; set; } = false;
    
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
