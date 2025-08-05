
namespace RentalService.Application.Rentals.Commands.Update;
using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Shared.ErrorHandling;
using Microsoft.Extensions.Logging;

public class UpdateRentalCommandHandler(RentalDbContext dbContext, IMapper mapper, ILogger<UpdateRentalCommandHandler> logger)
    : IRequestHandler<UpdateRentalCommand>
{
    public async Task Handle(UpdateRentalCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug("UpdateRentalCommand received for RentalId: {RentalId}", request.Id);
        var entity = await dbContext.Rentals.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (entity == null)
        {
            logger.LogDebug("Rental not found for Id: {RentalId}", request.Id);
            throw new KeyNotFoundException("Rental not found.");
        }
        if (entity.Status != RentalService.Contracts.Enums.RentalStatus.Pending)
        {
            logger.LogDebug("Update blocked for RentalId: {RentalId} due to status: {Status}", request.Id, entity.Status);
            throw new BusinessRuleException("Only rentals with status 'Pending' can be updated.");
        }
        
        request.StartDate = request.StartDate.Date;
        request.EndDate = request.EndDate.Date;
        request.BookingDate = request.BookingDate.Date;
        mapper.Map(request, entity);
        logger.LogDebug("Rental with Id: {RentalId} mapped with update request.", request.Id);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogDebug("DbContext changes saved for updated RentalId: {RentalId}", request.Id);
    }
}