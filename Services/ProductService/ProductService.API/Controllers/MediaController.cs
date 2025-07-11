using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Products.Commands.AddProductMedia;
using ProductService.Application.Products.Queries.GetProductMedia;
using ProductService.Contracts.DTOs;

namespace ProductService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get all media for a specific product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="color">Filter by color (optional)</param>
    /// <returns>List of product media</returns>
    [HttpGet("product/{productId:guid}")]
    public async Task<ActionResult<List<ProductMediaDto>>> GetProductMedia(
        Guid productId,
        [FromQuery] string? color = null)
    {
        var query = new GetProductMediaQuery(productId, color);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get media organized by color for a product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <returns>Media organized by color</returns>
    [HttpGet("product/{productId:guid}/by-color")]
    public async Task<ActionResult<Dictionary<string, List<ProductMediaDto>>>> GetProductMediaByColor(Guid productId)
    {
        var query = new GetProductMediaQuery(productId);
        var allMedia = await mediator.Send(query);
        
        var mediaByColor = allMedia
            .Where(m => !m.IsGeneric && !string.IsNullOrEmpty(m.Color))
            .GroupBy(m => m.Color!)
            .ToDictionary(g => g.Key, g => g.ToList());
            
        var genericMedia = allMedia.Where(m => m.IsGeneric).ToList();
        if (genericMedia.Any())
        {
            mediaByColor["Generic"] = genericMedia;
        }
        
        return Ok(mediaByColor);
    }

    /// <summary>
    /// Add new media to a product
    /// </summary>
    /// <param name="command">Add product media command</param>
    /// <returns>Created media ID</returns>
    [HttpPost]
    public async Task<ActionResult<Guid>> AddProductMedia([FromBody] AddProductMediaCommand command)
    {
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetProductMedia), new { productId = command.ProductId }, result);
    }

    /// <summary>
    /// Get the main/primary image for a product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="color">Specific color (optional)</param>
    /// <returns>Main product image</returns>
    [HttpGet("product/{productId:guid}/main")]
    public async Task<ActionResult<ProductMediaDto>> GetMainProductImage(
        Guid productId, 
        [FromQuery] string? color = null)
    {
        var query = new GetProductMediaQuery(productId, color);
        var media = await mediator.Send(query);
        
        var mainImage = media.FirstOrDefault(m => m.MediaPurpose == "main");
        if (mainImage == null)
        {
            // Fallback to any image if no main image is found
            mainImage = media.FirstOrDefault();
        }
        
        if (mainImage == null)
            return NotFound($"No media found for product {productId}");
            
        return Ok(mainImage);
    }

    /// <summary>
    /// Get product gallery images (excluding main image)
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="color">Filter by color (optional)</param>
    /// <returns>Gallery images</returns>
    [HttpGet("product/{productId:guid}/gallery")]
    public async Task<ActionResult<List<ProductMediaDto>>> GetProductGallery(
        Guid productId,
        [FromQuery] string? color = null)
    {
        var query = new GetProductMediaQuery(productId, color);
        var media = await mediator.Send(query);
        
        // Exclude main images from gallery
        var galleryImages = media.Where(m => m.MediaPurpose != "main").ToList();
        
        return Ok(galleryImages);
    }

    /// <summary>
    /// Delete product media by ID
    /// </summary>
    /// <param name="mediaId">Media ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{mediaId:guid}")]
    public ActionResult DeleteProductMedia(Guid mediaId)
    {
        // For now, we'll need to implement a DeleteProductMediaCommand
        // This is a placeholder implementation
        return BadRequest("Delete media functionality not yet implemented. Please create DeleteProductMediaCommand.");
    }
}
