using MediatR;
using CustomerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Addresses.Commands.Delete;

public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, bool>
{
    private readonly CustomerDbContext _context;

    public DeleteAddressCommandHandler(CustomerDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (address == null) return false;

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}