using RentalService.Infrastructure.Persistence;
using RentalService.Domain.Entities;
using RentalService.Contracts.Enums;
using MediatR;
using RentalService.Application.Notifications.Commands;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using RentalService.Contracts.DTOs;
using Microsoft.Extensions.Logging;
using NotificationService.Contracts.Enums;
using RentalService.Application.Rentals.Queries.GetRentalsWithDetails;

namespace RentalService.Application.Jobs.Commands.SendAdminReturnDeliverySummary;

public class SendAdminReturnDeliverySummaryCommandHandler : IRequestHandler<SendAdminReturnDeliverySummaryCommand>
{
    private readonly ILogger<SendAdminReturnDeliverySummaryCommandHandler> _logger;
    private readonly RentalDbContext _dbContext;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly string _baseUrl;

    public SendAdminReturnDeliverySummaryCommandHandler(
        ILogger<SendAdminReturnDeliverySummaryCommandHandler> logger,
        RentalDbContext dbContext,
        IMediator mediator,
        IMapper mapper,
        IConfiguration configuration)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mediator = mediator;
        _mapper = mapper;
        _baseUrl = configuration["App:AdminPortalBaseUrl"] ?? "https://www.saanjhicreation.com";
    }

    public async Task Handle(SendAdminReturnDeliverySummaryCommand request, CancellationToken cancellationToken)
    {
        using var scope = _logger.BeginScope("SendAdminReturnDeliverySummaryJob");
        _logger.LogTrace("Starting admin return/delivery summary notification job");
        var today = DateTime.UtcNow.Date;
        var dueReturnsQueryable = _dbContext.Rentals
            .Where(r => r.EndDate.Date == today && r.Status == RentalStatus.PickedUp);
        var dueReturns = await _mediator.Send(new GetRentalsWithDetailsQuery() { Queryable = dueReturnsQueryable});
        var dueDeliveriesQueryable = _dbContext.Rentals
            .Where(r => r.StartDate.Date == today && r.Status == RentalStatus.Booked);
        var dueDeliveries = await _mediator.Send(new GetRentalsWithDetailsQuery() { Queryable = dueDeliveriesQueryable});
        _logger.LogInformation("Found {ReturnCount} returns and {DeliveryCount} deliveries due today", dueReturns?.Count, dueDeliveries.Count);
        // Send a summary notification to admin (customize as needed)
        await _mediator.Send(new SendRentalNotificationCommand
        {
            Rental = new { ReturnCount = dueReturns.Count, DeliveryCount = dueDeliveries.Count, Link = $"{_baseUrl.TrimEnd('/')}/calendar" },
            Type = NotificationType.AdminReturnDeliverySummary,
            // Optionally, add summary info to Metadata
        }, cancellationToken);
        _logger.LogInformation("Admin return/delivery summary notification job completed. {Total} rentals summarized.", dueReturns.Count + dueDeliveries.Count);
    }
}
