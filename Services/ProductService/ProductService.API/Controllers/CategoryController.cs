using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Categories.Commands.CreateCategory;
using ProductService.Application.Categories.Commands.UpdateCategory;
using ProductService.Application.Categories.Commands.DeleteCategory;
using ProductService.Application.Categories.Queries.GetAllCategories;
using ProductService.Application.Categories.Queries.GetCategoriesByIds;
using ProductService.Application.Categories.Queries.GetCategoryById;
using ProductService.Contracts.DTOs;

namespace ProductService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get all categories
    /// </summary>
    /// <returns>List of all categories</returns>
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAllCategories()
    {
        var query = new GetAllCategoriesQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>Category details</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> GetCategoryById(Guid id)
    {
        var query = new GetCategoryByIdQuery { Id = id };
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get categories by IDs
    /// </summary>
    /// <param name="ids">List of category IDs</param>
    /// <returns>List of categories</returns>
    [HttpPost("by-ids")]
    public async Task<ActionResult<List<CategoryDto>>> GetCategoriesByIds([FromBody] List<Guid> ids)
    {
        var query = new GetCategoriesByIdsQuery { CategoryIds = ids };
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    /// <param name="command">Create category command</param>
    /// <returns>Created category ID</returns>
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateCategory([FromBody] CreateCategoryCommand command)
    {
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetCategoryById), new { id = result }, result);
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="command">Update category command</param>
    /// <returns>No content</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryCommand command)
    {
        command.Id = id;
        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteCategory(Guid id)
    {
        var command = new DeleteCategoryCommand { Id = id };
        await mediator.Send(command);
        return NoContent();
    }
}
