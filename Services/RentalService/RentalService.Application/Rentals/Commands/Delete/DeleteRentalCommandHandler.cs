namespace RentalService.Application.Rentals.Commands.Delete;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;

public class DeleteRentalCommandHandler(RentalDbContext dbContext) : IRequestHandler<DeleteRentalCommand>
{
    public async Task Handle(DeleteRentalCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Rentals.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (entity == null) throw new KeyNotFoundException("Rental not found.");

        dbContext.Rentals.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}