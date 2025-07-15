using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Categories.Commands.CreateCategory;
using ProductService.Application.Categories.Commands.UpdateCategory;
using ProductService.Application.Categories.Commands.DeleteCategory;
using ProductService.Application.Categories.Commands.RestoreCategory;
using ProductService.Application.Categories.Queries.GetAllCategories;
using ProductService.Application.Categories.Queries.GetCategoriesByIds;
using ProductService.Application.Categories.Queries.GetCategoryById;
using ProductService.Application.Categories.Queries.GetDeletedCategories;
using ProductService.Contracts.DTOs;
using Shared.Contracts.Common;

namespace ProductService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get all categories with optional search, sorting, and pagination
    /// </summary>
    /// <param name="search">General search term to filter categories by name or description</param>
    /// <param name="searchName">Search specifically in category name</param>
    /// <param name="searchDescription">Search specifically in category description</param>
    /// <param name="sortBy">Field to sort by (name, description, createdat, modifiedat)</param>
    /// <param name="sortDesc">Sort in descending order if true</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 10)</param>
    /// <returns>Paginated list of categories</returns>
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<CategoryDto>>> GetAllCategories(
        [FromQuery] string? search = null,
        [FromQuery] string? searchName = null,
        [FromQuery] string? searchDescription = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetAllCategoriesQuery
        {
            Search = search,
            SearchName = searchName,
            SearchDescription = searchDescription,
            SortBy = sortBy,
            SortDesc = sortDesc,
            Page = page,
            PageSize = pageSize
        };
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

    /// <summary>
    /// Get all deleted categories
    /// </summary>
    /// <returns>List of deleted categories</returns>
    [HttpGet("deleted")]
    public async Task<ActionResult<List<CategoryDto>>> GetDeletedCategories()
    {
        var query = new GetDeletedCategoriesQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Restore a soft deleted category (Admin only)
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>No content</returns>
    [HttpPost("{id:guid}/restore")]
    public async Task<ActionResult> RestoreCategory(Guid id)
    {
        var command = new RestoreCategoryCommand(id);
        await mediator.Send(command);
        return NoContent();
    }
}
