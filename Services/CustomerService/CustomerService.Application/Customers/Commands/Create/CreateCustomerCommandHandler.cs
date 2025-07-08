// File: Services/CustomerService/CustomerService.Application/Customers/Commands/CreateCustomerCommandHandler.cs

using CustomerService.Domain.Entities;
using CustomerService.Infrastructure.Persistence;
using MediatR;

namespace CustomerService.Application.Customers.Commands.Create;

public class CreateCustomerCommandHandler(CustomerDbContext context) : IRequestHandler<CreateCustomerCommand, Guid>
{
    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
}