using MediatR;
using Shared.Contracts.Categories;

namespace CategoryService.Application.Commands;

/// <summary>
/// Command to update a category.
/// </summary>
public class UpdateCategoryCommand : IRequest<CategoryDto>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
