using MediatR;
using Shared.Contracts.Categories;

namespace CategoryService.Application.Commands;

/// <summary>
/// Command to create a new category.
/// </summary>
public class CreateCategoryCommand : IRequest<CategoryDto>
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
