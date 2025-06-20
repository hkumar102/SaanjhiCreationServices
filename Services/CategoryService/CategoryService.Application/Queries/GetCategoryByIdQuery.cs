using MediatR;
using Shared.Contracts.Categories;

namespace CategoryService.Application.Queries;

/// <summary>
/// Query to get a category by Id.
/// </summary>
public class GetCategoryByIdQuery : IRequest<CategoryDto>
{
    public Guid Id { get; set; }
}
