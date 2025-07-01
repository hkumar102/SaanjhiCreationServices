using MediatR;
using CustomerService.Infrastructure.Persistence;
using CustomerService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Addresses.Queries.GetAddressById;

public class GetAddressByIdQueryHandler : IRequestHandler<GetAddressByIdQuery, Address?>
{
    private readonly CustomerDbContext _context;

    public GetAddressByIdQueryHandler(CustomerDbContext context)
    {
        _context = context;
    }

    public async Task<Address?> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
    }
}