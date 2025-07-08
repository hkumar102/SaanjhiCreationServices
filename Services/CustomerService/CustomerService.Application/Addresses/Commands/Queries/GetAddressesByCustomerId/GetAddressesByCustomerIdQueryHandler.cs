using CustomerService.Domain.Entities;
using CustomerService.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Addresses.Commands.Queries.GetAddressesByCustomerId;

public class GetAddressesByCustomerIdQueryHandler(CustomerDbContext context)
    : IRequestHandler<GetAddressesByCustomerIdQuery, List<Address>>
{
    public async Task<List<Address>> Handle(GetAddressesByCustomerIdQuery request, CancellationToken cancellationToken)
    {
        return await context.Addresses
            .Where(a => a.CustomerId == request.CustomerId)
            .ToListAsync(cancellationToken);
    }
}