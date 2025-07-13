using MediatR;
using Microsoft.AspNetCore.Http;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Media.Commands.UploadProductMedia;

public record UploadProductMediaCommand(
    Guid ProductId,
    IFormFile File,
    bool IsPrimary = false,
    string? Color = null,
    string? AltText = null,
    int DisplayOrder = 0
) : IRequest<ProductMediaDto>;
