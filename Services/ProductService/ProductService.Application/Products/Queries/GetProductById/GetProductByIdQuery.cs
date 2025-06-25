using MediatR;
using Shared.Contracts.Products;

namespace ProductService.Application.Products.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<ProductDto>
{
    public Guid Id { get; set; }
}
