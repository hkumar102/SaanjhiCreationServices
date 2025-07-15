using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Products.Queries.GetProductMedia;

public record GetProductMediaQuery(
    Guid ProductId,
    string? Color = null
) : IRequest<List<ProductMediaDto>>;
