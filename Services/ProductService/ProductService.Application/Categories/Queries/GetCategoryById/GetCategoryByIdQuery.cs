using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQuery : IRequest<CategoryDto>
{
    public Guid Id { get; set; }
}
