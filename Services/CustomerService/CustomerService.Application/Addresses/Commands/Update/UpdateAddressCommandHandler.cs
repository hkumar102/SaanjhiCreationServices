using MediatR;
using CustomerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Addresses.Commands.Update;

public class UpdateAddressCommandHandler(CustomerDbContext context) : IRequestHandler<UpdateAddressCommand, bool>
{
    public async Task<bool> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await context.Addresses.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
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

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}