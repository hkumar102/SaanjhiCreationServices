using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Categories.Queries.GetCategoriesByIds;

public class GetCategoriesByIdsQuery : IRequest<List<CategoryDto>>
{
    public List<Guid> CategoryIds { get; set; } = new();
}
