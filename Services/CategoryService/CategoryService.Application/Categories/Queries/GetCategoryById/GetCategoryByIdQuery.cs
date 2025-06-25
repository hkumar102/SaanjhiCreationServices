using MediatR;
using Shared.Contracts.Categories;

namespace CategoryService.Application.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQuery : IRequest<CategoryDto>
{
    public Guid Id { get; set; }
}
