using MediatR;
using ProductService.Contracts.DTOs;
using Shared.Contracts.Media;

namespace ProductService.Application.Media.Commands.AddProductMedia;

public record AddProductMediaCommand(
    Guid ProductId,
    string Url,
    MediaType MediaType,
    string? Color = null,
    string? Purpose = null,
    int SortOrder = 0,
    bool IsGeneric = false,
    string? AltText = null,
    string? Caption = null
) : IRequest<ProductMediaDto>;
