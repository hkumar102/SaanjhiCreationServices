using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using CustomerService.Application.Addresses.Commands.Queries.GetAddressesByCustomerId;
using CustomerService.Application.Customers.Queries;
using CustomerService.Application.Customers.Commands.Create;
using CustomerService.Application.Customers.Commands.Delete;
using CustomerService.Application.Customers.Commands.Update;
using CustomerService.Contracts.DTOs;

namespace CustomerService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
    {
        var id = await mediator.Send(command);
        return Ok(id);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerDto), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var customer = await mediator.Send(new GetCustomerByIdQuery(){ Id = id });
        return Ok(customer);
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(List<CustomerDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? name,
        [FromQuery] string? email,
        [FromQuery] string? phoneNumber,
        [FromQuery] string? sortBy,
        [FromQuery] bool sortDesc = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetAllCustomersQuery
        {
            Name = name,
            Email = email,
            PhoneNumber = phoneNumber,
            SortBy = sortBy,
            SortDesc = sortDesc,
            Page = page,
            PageSize = pageSize
        };
    
        var customers = await mediator.Send(query);
        return Ok(customers);
    }
    

    [HttpGet("{id}/Addresses")]
    public async Task<ActionResult<List<AddressDto>>> GetByCustomerId(Guid customerId)
        => Ok(await mediator.Send(new GetAddressesByCustomerIdQuery { CustomerId = customerId }));

    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> Update([FromBody] UpdateCustomerCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await mediator.Send(new DeleteCustomerCommand(){ Id = id });
        return NoContent();
    }
}