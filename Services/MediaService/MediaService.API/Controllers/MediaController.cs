using MediaService.Contracts.DTOs;
using MediaService.Contracts.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MediaService.Application.Commands;
using System.ComponentModel.DataAnnotations;

namespace MediaService.API.Controllers;

// MediaService.API/Controllers/MediaController.cs
[ApiController]
[Route("api/media")]
public class MediaController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Upload and process product images with automatic compression and scaling
    /// </summary>
    [HttpPost("product/upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ProductMediaUploadResult), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<ProductMediaUploadResult>> UploadProductMedia(IFormFile file, string? color = null, string? altText = null, bool isPrimary = false)
    {
        var command = new UploadProductMediaCommand
        {
            File = file,
            Color = color,
            AltText = altText,
            IsPrimary = isPrimary,
            ProcessingOptions = ProductImageProcessingOptions.Default
        };

        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Batch upload multiple product images
    /// </summary>
    [HttpPost("product/batch-upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(List<ProductMediaUploadResult>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<List<ProductMediaUploadResult>>> BatchUploadProductMedia(IFormFileCollection files, string? color = null)
    {
        var command = new BatchUploadProductMediaCommand
        {
            Files = files,
            Color = color,
            ProcessingOptions = ProductImageProcessingOptions.Default
        };

        var results = await mediator.Send(command);
        return Ok(results);
    }

    /// <summary>
    /// Legacy upload endpoint
    /// </summary>
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(UploadMediaResult), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<UploadMediaResult>> Upload(IFormFile file, string? productId = null, string? alt = null, bool isPrimary = false)
    {
        var result = await mediator.Send(new UploadMediaCommand
        {
            File = file,
            MediaType = MediaType.Image
        });

        return Ok(result);
    }

    /// <summary>
    /// Serve uploaded files
    /// </summary>
    [HttpGet("files/{category}/{fileName}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public IActionResult GetFile(string category, string fileName)
    {
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", category);
        var filePath = Path.Combine(uploadsPath, fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        // Security check: ensure the resolved path is within the uploads directory
        var fullUploadsPath = Path.GetFullPath(uploadsPath);
        var fullFilePath = Path.GetFullPath(filePath);
        
        if (!fullFilePath.StartsWith(fullUploadsPath))
        {
            return BadRequest("Invalid file path");
        }

        var contentType = GetContentType(fileName);
        return PhysicalFile(fullFilePath, contentType);
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream"
        };
    }
}
