// File: Services/CustomerService/CustomerService.Application/Customers/Commands/CreateCustomerCommandHandler.cs

using CustomerService.Domain.Entities;
using CustomerService.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.ErrorHandling;

namespace CustomerService.Application.Customers.Commands.Create;

public class CreateCustomerCommandHandler(
    CustomerDbContext context,
    ILogger<CreateCustomerCommandHandler> logger) : IRequestHandler<CreateCustomerCommand, Guid>
{
    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting CreateCustomerCommand execution for customer: {CustomerName}, Email: {Email}",
            request.Name, request.Email);

        try
        {

            // Customer with same phone or email cannot be created
            if (await context.Customers.AnyAsync(c => c.PhoneNumber == request.PhoneNumber ||
                                                      c.Email == request.Email, cancellationToken))
            {
                logger.LogWarning("Customer with PhoneNumber: {PhoneNumber} or Email: {Email} already exists",
                    request.PhoneNumber, request.Email);
                throw new BusinessRuleException("Customer with same phone or email already exists.");
            }


            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow
            };

            logger.LogDebug("Created customer entity with ID: {CustomerId}", customer.Id);

            context.Customers.Add(customer);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogDebug("Successfully created customer with ID: {CustomerId}", customer.Id);

            return customer.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating customer: {CustomerName}, Email: {Email}",
                request.Name, request.Email);
            throw;
        }
    }
}
