using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Notifications.Commands;
using NotificationService.Contracts.DTOs;

namespace RentalService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("push")]
    public async Task<ActionResult<NotificationDto>> SendPushNotification(
        [FromBody] SendPushNotificationCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
