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

namespace CategoryService.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoryController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll()
        => Ok(await mediator.Send(new GetAllCategoriesQuery()));

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
