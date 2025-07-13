using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Products.Commands.CreateProduct;
using ProductService.Application.Products.Commands.UpdateProduct;
using ProductService.Application.Products.Commands.DeleteProduct;
using ProductService.Application.Products.Queries.GetProductById;
using ProductService.Application.Products.Queries.GetAllProducts;
using ProductService.Application.Products.Queries.GetProductsByIds;
using ProductService.Application.Media.Commands.UploadProductMedia;
using ProductService.Contracts.DTOs;
using Shared.Contracts.Common;

namespace ProductService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IMediator mediator) : ControllerBase
{
    [HttpPost("search")]
    public async Task<ActionResult<PaginatedResult<ProductDto>>> Search([FromBody] GetAllProductsQuery query)
        => Ok(await mediator.Send(query));

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
        => Ok(await mediator.Send(new GetProductByIdQuery { Id = id }));

    [HttpPost("by-ids")]
    public async Task<ActionResult<List<ProductDto>>> GetByIds(GetProductsByIdsQuery query)
        => Ok(await mediator.Send(query));

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateProductCommand command)
        => Ok(await mediator.Send(command));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateProductCommand command)
    {
        if (id != command.Id)
            return BadRequest("Mismatched product ID");
        await mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await mediator.Send(new DeleteProductCommand { Id = id });
        return NoContent();
    }

    [HttpPost("{id}/media")]
    public async Task<ActionResult<ProductMediaDto>> UploadMedia(
        Guid id, 
        IFormFile file,
        [FromForm] bool isPrimary = false,
        [FromForm] string? color = null,
        [FromForm] string? altText = null,
        [FromForm] int displayOrder = 0)
    {
        var command = new UploadProductMediaCommand(id, file, isPrimary, color, altText, displayOrder);
        var result = await mediator.Send(command);
        return Ok(result);
    }
}
