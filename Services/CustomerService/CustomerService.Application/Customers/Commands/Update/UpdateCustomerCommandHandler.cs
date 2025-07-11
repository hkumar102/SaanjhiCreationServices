using MediatR;
using Microsoft.Extensions.Logging;
using CustomerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Customers.Commands.Update;

public class UpdateCustomerCommandHandler(
    CustomerDbContext context,
    ILogger<UpdateCustomerCommandHandler> logger) : IRequestHandler<UpdateCustomerCommand, bool>
{
    public async Task<bool> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting {HandlerName} execution for customer ID: {CustomerId}", 
            nameof(UpdateCustomerCommandHandler), request.Id);

        try
        {
            logger.LogDebug("Fetching customer with ID: {CustomerId}", request.Id);
            var customer = await context.Customers.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            
            if (customer == null)
            {
                logger.LogDebug("Customer not found with ID: {CustomerId}", request.Id);
                return false;
            }

            logger.LogDebug("Updating customer: {CustomerId} with new data", request.Id);
            customer.Name = request.Name;
            customer.Email = request.Email;
            customer.PhoneNumber = request.PhoneNumber;
            customer.UserId = request.UserId;
            customer.ModifiedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(cancellationToken);

            logger.LogDebug("{HandlerName} completed successfully for customer ID: {CustomerId}", 
                nameof(UpdateCustomerCommandHandler), request.Id);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing {HandlerName} for customer ID: {CustomerId}", 
                nameof(UpdateCustomerCommandHandler), request.Id);
            throw;
        }
    }
}
