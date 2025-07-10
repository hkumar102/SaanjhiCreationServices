using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Products.Queries.GetProductsByIds;

public class GetProductsByIdsQuery : IRequest<List<ProductDto>>
{
    public List<Guid> ProductIds { get; set; } = new();
}
