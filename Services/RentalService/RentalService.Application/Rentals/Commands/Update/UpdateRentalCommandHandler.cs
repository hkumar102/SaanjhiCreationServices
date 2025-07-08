namespace RentalService.Application.Rentals.Commands.Update;

using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;

public class UpdateRentalCommandHandler(RentalDbContext dbContext, IMapper mapper)
    : IRequestHandler<UpdateRentalCommand>
{
    public async Task Handle(UpdateRentalCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Rentals.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (entity == null) throw new KeyNotFoundException("Rental not found.");

        mapper.Map(request, entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}