using MediatR;
using CategoryService.Contracts.DTOs;

namespace CategoryService.Application.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQuery : IRequest<CategoryDto>
{
    public Guid Id { get; set; }
}
