using AutoMapper;
using CustomerService.Contracts.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;
using CustomerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Customers.Queries;

public class GetCustomerByIdQueryHandler(
    CustomerDbContext context, 
    IMapper mapper,
    ILogger<GetCustomerByIdQueryHandler> logger)
    : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetCustomerByIdQuery execution for CustomerId: {CustomerId}", request.Id);

        try
        {
            logger.LogDebug("Fetching customer with ID: {CustomerId} from database", request.Id);
            var customer = await context.Customers
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (customer == null)
            {
                logger.LogDebug("Customer with ID: {CustomerId} not found", request.Id);
                return null;
            }

            logger.LogDebug("Found customer: {CustomerName}, AddressCount: {AddressCount}", customer.Name, customer.Addresses.Count);

            var result = mapper.Map<CustomerDto>(customer);
            
            logger.LogDebug("GetCustomerByIdQuery completed successfully for CustomerId: {CustomerId}", request.Id);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing GetCustomerByIdQuery for CustomerId: {CustomerId}", request.Id);
            throw;
        }
    }
}
