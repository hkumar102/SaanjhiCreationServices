using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalService.Application.Rentals.Commands.Create;
using RentalService.Application.Rentals.Commands.Delete;
using RentalService.Application.Rentals.Commands.Update;
using RentalService.Application.Rentals.Queries.GetRentalById;
using RentalService.Application.Rentals.Queries.GetRentals;
using RentalService.Contracts.DTOs;
using Shared.Contracts.Common;

namespace RentalService.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class RentalController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<RentalDto>>> GetRentals([FromQuery] GetRentalsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RentalDto>> GetRentalById(Guid id)
    {
        var result = await mediator.Send(new GetRentalByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateRental([FromBody] CreateRentalCommand command)
    {
        var id = await mediator.Send(command);
        return Ok(id);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateRental(Guid id, [FromBody] UpdateRentalCommand command)
    {
        if (id != command.Id) return BadRequest("Mismatched Rental ID");
        await mediator.Send(command);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRental(Guid id)
    {
        await mediator.Send(new DeleteRentalCommand { Id = id });
        return Ok();
    }
}