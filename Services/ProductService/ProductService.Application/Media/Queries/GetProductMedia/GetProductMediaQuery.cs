using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Media.Queries.GetProductMedia;

public record GetProductMediaQuery(
    Guid ProductId,
    string? Color = null,
    string? Purpose = null,
    bool IncludeGeneric = true
) : IRequest<List<ProductMediaDto>>;
