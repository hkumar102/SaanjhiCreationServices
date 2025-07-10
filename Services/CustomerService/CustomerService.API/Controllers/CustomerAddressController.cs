using System.Net;
using CustomerService.Application.Addresses.Commands.Create;
using CustomerService.Application.Addresses.Commands.Delete;
using CustomerService.Application.Addresses.Commands.Queries.GetAddressById;
using CustomerService.Application.Addresses.Commands.Queries.GetAddressesByIds;
using CustomerService.Application.Addresses.Commands.Update;
using CustomerService.Contracts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerAddressController(IMediator mediator) : ControllerBase
{
    
    [HttpGet("{id}")]
    public async Task<ActionResult<AddressDto>> GetById(Guid id)
        => Ok(await mediator.Send(new GetAddressByIdQuery { Id = id }));

    [HttpPost("by-ids")]
    [ProducesResponseType(typeof(List<AddressDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetByIds([FromBody] GetAddressesByIdsQuery query)
    {
        var addresses = await mediator.Send(query);
        return Ok(addresses);
    }
    
    [HttpPost("address")]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> AddAddress([FromBody] CreateAddressCommand command)
    {
        var id = await mediator.Send(command);
        return Ok(id);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateAddressCommand command)
    {
        if (id != command.Id)
            return BadRequest("Mismatched address ID");
        await mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await mediator.Send(new DeleteAddressCommand { Id = id });
        return NoContent();
    }
}