using MediatR;
using CustomerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Customers.Commands.Delete;

public class DeleteCustomerCommandHandler(CustomerDbContext context) : IRequestHandler<DeleteCustomerCommand, bool>
{
    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await context.Customers.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (customer == null) return false;

        context.Customers.Remove(customer);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}