using AutoMapper;
using CustomerService.Contracts.DTOs;
using MediatR;
using CustomerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Customers.Queries;

public class GetCustomerByIdQueryHandler(CustomerDbContext context, IMapper mapper)
    : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await context.Customers
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        return customer == null ? null : mapper.Map<CustomerDto>(customer);
    }
}