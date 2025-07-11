using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Products.Commands.AddInventoryItem;
using ProductService.Application.Products.Commands.UpdateInventoryStatus;
using ProductService.Application.Products.Queries.GetProductInventory;
using ProductService.Contracts.DTOs;

namespace ProductService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get inventory items for a specific product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="size">Size filter (optional)</param>
    /// <param name="color">Color filter (optional)</param>
    /// <param name="includeRetired">Include retired items (optional)</param>
    /// <returns>List of inventory items for the product</returns>
    [HttpGet("product/{productId:guid}")]
    public async Task<ActionResult<List<InventoryItemDto>>> GetProductInventory(
        Guid productId,
        [FromQuery] string? size = null,
        [FromQuery] string? color = null,
        [FromQuery] bool includeRetired = false)
    {
        var query = new GetProductInventoryQuery(productId, size, color, null, includeRetired);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Add a new inventory item to a product
    /// </summary>
    /// <param name="command">Add inventory item command</param>
    /// <returns>Created inventory item ID</returns>
    [HttpPost]
    public async Task<ActionResult<Guid>> AddInventoryItem([FromBody] AddInventoryItemCommand command)
    {
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetProductInventory), new { productId = command.ProductId }, result);
    }

    /// <summary>
    /// Update the status of an inventory item
    /// </summary>
    /// <param name="inventoryItemId">Inventory item ID</param>
    /// <param name="command">Update inventory status command</param>
    /// <returns>No content</returns>
    [HttpPut("{inventoryItemId:guid}/status")]
    public async Task<ActionResult> UpdateInventoryStatus(Guid inventoryItemId, [FromBody] UpdateInventoryStatusCommand command)
    {
        if (inventoryItemId != command.InventoryItemId)
            return BadRequest("Mismatched inventory item ID");
            
        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Get inventory item by ID
    /// </summary>
    /// <param name="inventoryItemId">Inventory item ID</param>
    /// <returns>Inventory item details</returns>
    [HttpGet("{inventoryItemId:guid}")]
    public ActionResult<InventoryItemDto> GetInventoryItem(Guid inventoryItemId)
    {
        // We need to find which product this inventory item belongs to
        // For now, this is a simplified approach - in production you might want a dedicated query
        return BadRequest("Use GET /api/inventory/product/{productId} to get inventory items for a specific product");
    }

    /// <summary>
    /// Get available inventory items for a product by size and color
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="size">Size filter (optional)</param>
    /// <param name="color">Color filter (optional)</param>
    /// <returns>Available inventory items</returns>
    [HttpGet("product/{productId:guid}/available")]
    public async Task<ActionResult<List<InventoryItemDto>>> GetAvailableInventory(
        Guid productId, 
        [FromQuery] string? size = null, 
        [FromQuery] string? color = null)
    {
        var query = new GetProductInventoryQuery(
            productId, 
            size, 
            color, 
            ProductService.Contracts.Enums.InventoryStatus.Available, 
            false);
        
        var result = await mediator.Send(query);
        return Ok(result);
    }
}
