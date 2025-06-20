using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.Common;
using UserService.Application.Commands;
using UserService.Application.Queries;
using Shared.Contracts.Users;

namespace UserService.API.Controllers;

/// <summary>
/// Controller for user operations.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new user.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetByUserId), new { userId = result.Id }, result);
    }

    /// <summary>
    /// Update user details.
    /// </summary>
    [HttpPut]
    public async Task<ActionResult<UserDto>> Update(UpdateUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Add a shipping address.
    /// </summary>
    [HttpPost("address")]
    public async Task<ActionResult<ShippingAddressDto>> AddAddress(AddShippingAddressCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deactivate a user.
    /// </summary>
    [HttpPost("{userId:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid userId)
    {
        await _mediator.Send(new DeactivateUserCommand { UserId = userId });
        return NoContent();
    }

    /// <summary>
    /// Activate a user.
    /// </summary>
    [HttpPost("{userId:guid}/activate")]
    public async Task<IActionResult> Activate(Guid userId)
    {
        await _mediator.Send(new ActivateUserCommand { UserId = userId });
        return NoContent();
    }

    /// <summary>
    /// Get a user by Firebase User ID.
    /// </summary>
    [HttpGet("firebase/{firebaseUserId}")]
    public async Task<ActionResult<UserDto>> GetByFirebaseUserId(string firebaseUserId)
    {
        var result = await _mediator.Send(new GetUserByFirebaseUserIdQuery { FirebaseUserId = firebaseUserId });
        return Ok(result);
    }

    /// <summary>
    /// Get a user by User ID.
    /// </summary>
    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<UserDto>> GetByUserId(Guid userId)
    {
        var result = await _mediator.Send(new GetUserByUserIdQuery { UserId = userId });
        return Ok(result);
    }

    /// <summary>
    /// Get roles for a user.
    /// </summary>
    [HttpGet("{userId:guid}/roles")]
    public async Task<ActionResult<List<string>>> GetRoles(Guid userId)
    {
        var result = await _mediator.Send(new GetUserRolesQuery { UserId = userId });
        return Ok(result);
    }

    /// <summary>
    /// Search users with filters.
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<PaginatedResult<UserDto>>> Search([FromQuery] SearchUsersQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
