using RentalService.Infrastructure.Persistence;
using RentalService.Domain.Entities;
using RentalService.Contracts.Enums;
using MediatR;
using RentalService.Application.Notifications.Commands;
using Microsoft.Extensions.Logging;
using AutoMapper;
using NotificationService.Contracts.Enums;
using RentalService.Contracts.DTOs;

namespace RentalService.Application.Jobs.Commands.AutoMarkOverdueRentals;

public class AutoMarkOverdueRentalsCommandHandler : IRequestHandler<AutoMarkOverdueRentalsCommand>
{
    private readonly ILogger<AutoMarkOverdueRentalsCommandHandler> _logger;
    private readonly RentalDbContext _dbContext;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public AutoMarkOverdueRentalsCommandHandler(
        ILogger<AutoMarkOverdueRentalsCommandHandler> logger,
        RentalDbContext dbContext,
        IMediator mediator,
        IMapper mapper)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task Handle(AutoMarkOverdueRentalsCommand request, CancellationToken cancellationToken)
    {
        using var scope = _logger.BeginScope("AutoMarkOverdueRentalsJob");
        _logger.LogTrace("Starting auto-mark overdue job for rentals past end date");
        var now = DateTime.UtcNow;
        var overdueRentals = _dbContext.Rentals
            .Where(r => r.Status != RentalStatus.Returned && r.Status != RentalStatus.Cancelled && r.EndDate < now)
            .ToList();
        _logger.LogInformation("Found {Count} rentals past end date and not returned/cancelled", overdueRentals.Count);
        foreach (var rental in overdueRentals)
        {
            _logger.LogTrace("Marking rental {RentalNumber} (Id: {RentalId}) as Overdue", rental.RentalNumber, rental.Id);
            rental.Status = RentalStatus.Overdue;
            _dbContext.RentalTimelines.Add(new RentalTimeline
            {
                RentalId = rental.Id,
                Status = (int)RentalStatus.Overdue,
                Notes = "Auto-marked as overdue by system."
            });
            await _mediator.Send(new SendRentalNotificationCommand
            {
                Rental = _mapper.Map<RentalDto>(rental),
                Type = NotificationType.RentalStatusChanged
            }, cancellationToken);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Auto-mark overdue job completed. {Count} rentals marked as overdue.", overdueRentals.Count);
    }
}
