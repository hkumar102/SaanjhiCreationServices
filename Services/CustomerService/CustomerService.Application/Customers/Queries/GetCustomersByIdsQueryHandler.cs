using AutoMapper;
using CustomerService.Contracts.DTOs;
using MediatR;
using CustomerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Customers.Queries;

public class GetCustomersByIdsQueryHandler(CustomerDbContext context, IMapper mapper)
    : IRequestHandler<GetCustomersByIdsQuery, List<CustomerDto>>
{
    public async Task<List<CustomerDto>> Handle(GetCustomersByIdsQuery request, CancellationToken cancellationToken)
    {
        if (!request.CustomerIds.Any())
            return new List<CustomerDto>();

        var customers = await context.Customers
            .Include(c => c.Addresses)
            .Where(c => request.CustomerIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        return mapper.Map<List<CustomerDto>>(customers);
    }
}
