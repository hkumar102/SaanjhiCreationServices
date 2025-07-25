using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Notifications.Commands;
using NotificationService.Contracts.DTOs;
using RentalService.Application.Jobs.Commands.AutoCancelPendingRentals;
using RentalService.Application.Jobs.Commands.AutoMarkOverdueRentals;
using RentalService.Application.Jobs.Commands.SendAdminReturnDeliveryAlert;
using RentalService.Application.Jobs.Commands.SendAdminReturnDeliverySummary;

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

    [HttpPost("auto-cancel-pending")]
    public async Task<IActionResult> AutoCancelPending(CancellationToken cancellationToken)
    {
        await _mediator.Send(new AutoCancelPendingRentalsCommand(), cancellationToken);
        return Ok();
    }

    [HttpPost("auto-mark-overdue")]
    public async Task<IActionResult> AutoMarkOverdue(CancellationToken cancellationToken)
    {
        await _mediator.Send(new AutoMarkOverdueRentalsCommand(), cancellationToken);
        return Ok();
    }

    [HttpPost("admin-return-delivery-alert")]
    public async Task<IActionResult> AdminReturnDeliveryAlert(CancellationToken cancellationToken)
    {
        await _mediator.Send(new SendAdminReturnDeliveryAlertCommand(), cancellationToken);
        return Ok();
    }

    [HttpPost("admin-return-delivery-summary")]
    public async Task<IActionResult> AdminReturnDeliverySummary(CancellationToken cancellationToken)
    {
        await _mediator.Send(new SendAdminReturnDeliverySummaryCommand(), cancellationToken);
        return Ok();
    }
}
