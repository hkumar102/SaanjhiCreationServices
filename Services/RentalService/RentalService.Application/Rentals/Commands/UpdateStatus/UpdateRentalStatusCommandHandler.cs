using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.ErrorHandling;
using Microsoft.Extensions.Logging;
using RentalService.Contracts.Enums;
using RentalService.Infrastructure.Persistence;

namespace RentalService.Application.Rentals.Commands.UpdateStatus;

public class UpdateRentalStatusCommandHandler : IRequestHandler<UpdateRentalStatusCommand>
{
    private readonly RentalDbContext dbContext;
    private readonly ILogger<UpdateRentalStatusCommandHandler> logger;

    public UpdateRentalStatusCommandHandler(
        RentalDbContext dbContext,
        ILogger<UpdateRentalStatusCommandHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task Handle(UpdateRentalStatusCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug("UpdateRentalStatusCommand received for RentalId: {RentalId} to Status: {Status}", request.Id, request.Status);
        var entity = await dbContext.Rentals.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (entity == null)
        {
            logger.LogDebug("Rental not found for Id: {RentalId}", request.Id);
            throw new KeyNotFoundException("Rental not found.");
        }

        if (!IsValidTransition(entity.Status, request.Status))
        {
            logger.LogDebug("Invalid status transition from {CurrentStatus} to {NextStatus} for RentalId: {RentalId}", entity.Status, request.Status, request.Id);
            throw new BusinessRuleException($"Cannot transition from {entity.Status} to {request.Status}.");
        }

        entity.Status = request.Status;
        if (!string.IsNullOrWhiteSpace(request.Notes))
            entity.Notes = request.Notes;

        if (request.Status == RentalStatus.PickedUp || request.Status == RentalStatus.Booked)
        {
            if (!request.ActualStartDate.HasValue)
                throw new BusinessRuleException("ActualStartDate is required when marking as PickedUp or Booked.");
            entity.ActualStartDate = request.ActualStartDate.Value;
        }
        if (request.Status == RentalStatus.Returned)
        {
            if (!request.ActualReturnDate.HasValue)
                throw new BusinessRuleException("ActualReturnDate is required when marking as Returned.");
            entity.ActualReturnDate = request.ActualReturnDate.Value;
        }

        // Add RentalTimeline entry for status change
        dbContext.RentalTimelines.Add(new RentalService.Domain.Entities.RentalTimeline
        {
            Id = Guid.NewGuid(),
            RentalId = entity.Id,
            Status = (int)request.Status,
            Notes = request.Notes
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogDebug("Rental status updated for RentalId: {RentalId} to Status: {Status}", request.Id, request.Status);
    }

    private static bool IsValidTransition(RentalStatus current, RentalStatus next)
    {
        return current switch
        {
            RentalStatus.Pending => next == RentalStatus.Booked || next == RentalStatus.Cancelled,
            RentalStatus.Booked => next == RentalStatus.PickedUp || next == RentalStatus.Cancelled,
            RentalStatus.PickedUp => next == RentalStatus.Returned || next == RentalStatus.Overdue,
            RentalStatus.Overdue => next == RentalStatus.Returned,
            RentalStatus.Returned => false,
            RentalStatus.Cancelled => false,
            _ => false
        };
    }
}
