using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalService.Application.Rentals.Commands.Create;
using RentalService.Application.Rentals.Commands.Delete;
using RentalService.Application.Rentals.Commands.Update;
using RentalService.Application.Rentals.Commands.UpdateStatus;
using RentalService.Application.Rentals.Queries.GetRentalById;
using RentalService.Application.Rentals.Queries.GetRentals;
using RentalService.Application.Rentals.Queries.GetRentalTimeline;
using RentalService.Contracts.DTOs;
using Shared.Contracts.Common;

namespace RentalService.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class RentalController(IMediator mediator) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<RentalDto>>> GetRentals(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetRentalsQuery
        {
            FromDate = fromDate,
            ToDate = toDate,
            Page = page,
            PageSize = pageSize
        };
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("search")]
    public async Task<ActionResult<PaginatedResult<RentalDto>>> Search([FromBody] GetRentalsQuery query)
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

    [HttpPut("{id}/status")]
    public async Task<ActionResult> UpdateRentalStatus(Guid id, [FromBody] UpdateRentalStatusCommand command)
    {
        if (id != command.Id) return BadRequest("Mismatched Rental ID");
        await mediator.Send(command);
        return Ok();
    }

    [HttpGet("{rentalId}/timeline")]
    public async Task<ActionResult<List<RentalTimelineDto>>> GetRentalTimeline(Guid rentalId)
    {
        var result = await mediator.Send(new GetRentalTimelineQuery(rentalId));
        return Ok(result);
    }
}