using CustomerService.Domain.Entities;
using CustomerService.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Addresses.Commands.Queries.GetAddressById;

public class GetAddressByIdQueryHandler(CustomerDbContext context) : IRequestHandler<GetAddressByIdQuery, Address?>
{
    public async Task<Address?> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
    {
        return await context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
    }
}