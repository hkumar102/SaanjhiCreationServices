using MediatR;
using CustomerService.Infrastructure.Persistence;
using CustomerService.Domain.Entities;

namespace CustomerService.Application.Addresses.Commands.Create;

public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, Guid>
{
    private readonly CustomerDbContext _context;

    public CreateAddressCommandHandler(CustomerDbContext context)
    {
        _context = context;
    }

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

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync(cancellationToken);
        return address.Id;
    }
}