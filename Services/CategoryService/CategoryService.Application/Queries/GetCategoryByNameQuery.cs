using MediatR;
using Shared.Contracts.Categories;

namespace CategoryService.Application.Queries;

/// <summary>
/// Query to get a category by Name.
/// </summary>
public class GetCategoryByNameQuery : IRequest<CategoryDto>
{
    public string Name { get; set; } = null!;
}
