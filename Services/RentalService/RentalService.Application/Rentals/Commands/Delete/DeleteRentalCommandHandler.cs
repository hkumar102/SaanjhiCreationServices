namespace RentalService.Application.Rentals.Commands.Delete;


using MediatR;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Shared.ErrorHandling;
using Microsoft.Extensions.Logging;

public class DeleteRentalCommandHandler(RentalDbContext dbContext, ILogger<DeleteRentalCommandHandler> logger) : IRequestHandler<DeleteRentalCommand>
{
    public async Task Handle(DeleteRentalCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug("DeleteRentalCommand received for RentalId: {RentalId}", request.Id);
        var entity = await dbContext.Rentals.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (entity == null)
        {
            logger.LogDebug("Rental not found for Id: {RentalId}", request.Id);
            throw new BusinessRuleException("Rental not found.");
        }
        if (entity.Status != RentalService.Contracts.Enums.RentalStatus.Pending && entity.Status != RentalService.Contracts.Enums.RentalStatus.Cancelled)
        {
            logger.LogDebug("Delete blocked for RentalId: {RentalId} due to status: {Status}", request.Id, entity.Status);
            throw new BusinessRuleException("Only rentals with status 'Pending' or 'Cancelled' can be deleted.");
        }

        dbContext.Rentals.Remove(entity);
        logger.LogDebug("Rental with Id: {RentalId} removed from DbContext.", request.Id);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogDebug("DbContext changes saved for deleted RentalId: {RentalId}", request.Id);
    }
}