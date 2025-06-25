using MediatR;
using Microsoft.AspNetCore.Mvc;
using CategoryService.Application.Categories.Commands.CreateCategory;
using CategoryService.Application.Categories.Commands.UpdateCategory;
using CategoryService.Application.Categories.Commands.DeleteCategory;
using CategoryService.Application.Categories.Queries.GetAllCategories;
using CategoryService.Application.Categories.Queries.GetCategoryById;
using Shared.Contracts.Categories;

namespace CategoryService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll()
        => Ok(await _mediator.Send(new GetAllCategoriesQuery()));

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id)
        => Ok(await _mediator.Send(new GetCategoryByIdQuery { Id = id }));

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateCategoryCommand command)
        => Ok(await _mediator.Send(command));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateCategoryCommand command)
    {
        if (id != command.Id)
            return BadRequest("Mismatched category ID");
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteCategoryCommand { Id = id });
        return NoContent();
    }
}
