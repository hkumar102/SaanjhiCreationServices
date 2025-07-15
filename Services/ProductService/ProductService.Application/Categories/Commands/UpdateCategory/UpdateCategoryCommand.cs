using MediatR;

namespace ProductService.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommand : IRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
