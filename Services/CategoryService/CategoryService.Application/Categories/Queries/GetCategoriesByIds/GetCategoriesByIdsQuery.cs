using MediatR;
using CategoryService.Contracts.DTOs;
namespace CategoryService.Application.Categories.Queries.GetCategoriesByIds;

public class GetCategoriesByIdsQuery : IRequest<List<CategoryDto>>
{
    public List<Guid> CategoryIds { get; set; } = new();
}