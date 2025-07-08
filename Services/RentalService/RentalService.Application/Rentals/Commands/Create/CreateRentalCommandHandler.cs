using RentalService.Contracts.Enums;

namespace RentalService.Application.Rentals.Commands.Create;

using MediatR;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Persistence;

public class CreateRentalCommandHandler(RentalDbContext dbContext, IMapper mapper)
    : IRequestHandler<CreateRentalCommand, Guid>
{
    public async Task<Guid> Handle(CreateRentalCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<Rental>(request);
        entity.Status = RentalStatus.Pending;

        dbContext.Rentals.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}