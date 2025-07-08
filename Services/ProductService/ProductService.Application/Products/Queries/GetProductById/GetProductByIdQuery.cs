using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Products.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<ProductDto>
{
    public Guid Id { get; set; }
}
