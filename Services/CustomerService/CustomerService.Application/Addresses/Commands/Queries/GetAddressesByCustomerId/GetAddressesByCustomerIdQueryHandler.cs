using MediatR;
using CustomerService.Infrastructure.Persistence;
using CustomerService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Addresses.Queries.GetAddressesByCustomerId;

public class GetAddressesByCustomerIdQueryHandler : IRequestHandler<GetAddressesByCustomerIdQuery, List<Address>>
{
    private readonly CustomerDbContext _context;

    public GetAddressesByCustomerIdQueryHandler(CustomerDbContext context)
    {
        _context = context;
    }

    public async Task<List<Address>> Handle(GetAddressesByCustomerIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Addresses
            .Where(a => a.CustomerId == request.CustomerId)
            .ToListAsync(cancellationToken);
    }
}