using MediatR;
using CustomerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Addresses.Commands.Delete;

public class DeleteAddressCommandHandler(CustomerDbContext context) : IRequestHandler<DeleteAddressCommand, bool>
{
    public async Task<bool> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await context.Addresses.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (address == null) return false;

        context.Addresses.Remove(address);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}