using MediatR;
using Microsoft.AspNetCore.Mvc;
using CategoryService.Application.Categories.Commands.CreateCategory;
using CategoryService.Application.Categories.Commands.UpdateCategory;
using CategoryService.Application.Categories.Commands.DeleteCategory;
using CategoryService.Application.Categories.Queries.GetAllCategories;
using CategoryService.Application.Categories.Queries.GetCategoriesByIds;
using CategoryService.Application.Categories.Queries.GetCategoryById;
using CategoryService.Contracts.DTOs;
using Microsoft.AspNetCore.Authorization;
using Shared.Contracts.Common;

namespace CategoryService.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoryController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<CategoryDto>>> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetAllCategoriesQuery
        {
            Search = search,
            SortBy = sortBy,
            SortDesc = sortDesc,
            Page = page,
            PageSize = pageSize
        };
        
        return Ok(await mediator.Send(query));
    }

    [HttpPost("search")]
    public async Task<ActionResult<PaginatedResult<CategoryDto>>> Search([FromBody] GetAllCategoriesQuery query)
        => Ok(await mediator.Send(query));

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id)
        => Ok(await mediator.Send(new GetCategoryByIdQuery { Id = id }));
    
    [HttpPost("by-ids")]
    public async Task<ActionResult<List<CategoryDto>>> GetByIds([FromBody] List<Guid> ids)
        => Ok(await mediator.Send(new GetCategoriesByIdsQuery { CategoryIds = ids }));

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateCategoryCommand command)
        => Ok(await mediator.Send(command));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateCategoryCommand command)
    {
        if (id != command.Id)
            return BadRequest("Mismatched category ID");
        await mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await mediator.Send(new DeleteCategoryCommand { Id = id });
        return NoContent();
    }
    
    
}
