using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Products.Commands.AddProductMedia;

public record AddProductMediaCommand(
    Guid ProductId,
    string Url,
    string MediaType,
    string? Color = null,
    int SortOrder = 0,
    bool IsGeneric = false,
    string? AltText = null
) : IRequest<ProductMediaDto>;
