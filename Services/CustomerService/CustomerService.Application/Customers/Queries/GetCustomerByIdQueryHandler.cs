using MediatR;
using CustomerService.Infrastructure.Persistence;
using CustomerService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Customers.Queries;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Customer?>
{
    private readonly CustomerDbContext _context;

    public GetCustomerByIdQueryHandler(CustomerDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Customers
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
    }
}