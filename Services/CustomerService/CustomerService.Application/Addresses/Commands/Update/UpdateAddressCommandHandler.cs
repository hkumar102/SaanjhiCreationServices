using MediatR;
using CustomerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Addresses.Commands.Update;

public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, bool>
{
    private readonly CustomerDbContext _context;

    public UpdateAddressCommandHandler(CustomerDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (address == null) return false;

        address.CustomerId = request.CustomerId;
        address.Line1 = request.Line1;
        address.Line2 = request.Line2;
        address.City = request.City;
        address.State = request.State;
        address.PostalCode = request.PostalCode;
        address.Country = request.Country;
        address.PhoneNumber = request.PhoneNumber;
        address.Type = request.Type;
        address.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}