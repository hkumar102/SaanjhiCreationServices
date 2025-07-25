using MediatR;
using Microsoft.Extensions.Logging;

namespace RentalService.Application.Jobs.Commands.SendAdminReturnDeliverySummary;

public class SendAdminReturnDeliverySummaryCommandHandler : IRequestHandler<SendAdminReturnDeliverySummaryCommand>
{
    private readonly ILogger<SendAdminReturnDeliverySummaryCommandHandler> _logger;

    public SendAdminReturnDeliverySummaryCommandHandler(ILogger<SendAdminReturnDeliverySummaryCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(SendAdminReturnDeliverySummaryCommand request, CancellationToken cancellationToken)
    {
        using var scope = _logger.BeginScope("SendAdminReturnDeliverySummaryJob");
        _logger.LogTrace("Starting admin return/delivery summary notification job");
        // TODO: Add business logic to send summary notification
        _logger.LogInformation("Admin return/delivery summary notification job completed");
    }
}
