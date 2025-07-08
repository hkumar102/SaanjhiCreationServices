using MediatR;
using CustomerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Customers.Commands.Update;

public class UpdateCustomerCommandHandler(CustomerDbContext context) : IRequestHandler<UpdateCustomerCommand, bool>
{
    public async Task<bool> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await context.Customers.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (customer == null) return false;

        customer.Name = request.Name;
        customer.Email = request.Email;
        customer.PhoneNumber = request.PhoneNumber;
        customer.UserId = request.UserId;
        customer.ModifiedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}