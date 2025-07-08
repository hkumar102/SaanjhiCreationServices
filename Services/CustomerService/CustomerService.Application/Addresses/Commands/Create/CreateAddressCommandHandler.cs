using MediatR;
using CustomerService.Infrastructure.Persistence;
using CustomerService.Domain.Entities;

namespace CustomerService.Application.Addresses.Commands.Create;

public class CreateAddressCommandHandler(CustomerDbContext context) : IRequestHandler<CreateAddressCommand, Guid>
{
    public async Task<Guid> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        var address = new Address
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            Line1 = request.Line1,
            Line2 = request.Line2,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Country = request.Country,
            PhoneNumber = request.PhoneNumber,
            Type = request.Type,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        context.Addresses.Add(address);
        await context.SaveChangesAsync(cancellationToken);
        return address.Id;
    }
}