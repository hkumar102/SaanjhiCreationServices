using MediatR;

namespace CategoryService.Application.Commands;

/// <summary>
/// Command to delete a category.
/// </summary>
public class DeleteCategoryCommand : IRequest
{
    public Guid Id { get; set; }
}
