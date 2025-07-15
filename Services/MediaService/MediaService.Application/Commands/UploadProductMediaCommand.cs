using MediatR;
using Microsoft.AspNetCore.Http;
using MediaService.Contracts.DTOs;

namespace MediaService.Application.Commands;

/// <summary>
/// Command to upload and process a single product image
/// </summary>
public class UploadProductMediaCommand : IRequest<ProductMediaUploadResult>
{
    public IFormFile File { get; set; } = null!;
    public string? Color { get; set; }
    public string? AltText { get; set; }
    public bool IsPrimary { get; set; }
    public ProductImageProcessingOptions ProcessingOptions { get; set; } = ProductImageProcessingOptions.Default;
}

/// <summary>
/// Command to batch upload multiple product images
/// </summary>
public class BatchUploadProductMediaCommand : IRequest<List<ProductMediaUploadResult>>
{
    public IFormFileCollection Files { get; set; } = null!;
    public string? Color { get; set; }
    public ProductImageProcessingOptions ProcessingOptions { get; set; } = ProductImageProcessingOptions.Default;
}
