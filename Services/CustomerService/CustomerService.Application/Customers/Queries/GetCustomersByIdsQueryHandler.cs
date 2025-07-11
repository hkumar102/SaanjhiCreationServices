using AutoMapper;
using Microsoft.Extensions.Logging;
using CustomerService.Contracts.DTOs;
using MediatR;
using CustomerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Customers.Queries;

public class GetCustomersByIdsQueryHandler(
    CustomerDbContext context, 
    IMapper mapper,
    ILogger<GetCustomersByIdsQueryHandler> logger)
    : IRequestHandler<GetCustomersByIdsQuery, List<CustomerDto>>
{
    public async Task<List<CustomerDto>> Handle(GetCustomersByIdsQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting {HandlerName} execution with {CustomerCount} customer IDs", 
            nameof(GetCustomersByIdsQueryHandler), request.CustomerIds.Count);

        try
        {
            if (!request.CustomerIds.Any())
            {
                logger.LogDebug("No customer IDs provided, returning empty list");
                return new List<CustomerDto>();
            }

            logger.LogDebug("Fetching customers with IDs: {CustomerIds}", request.CustomerIds);
            var customers = await context.Customers
                .Include(c => c.Addresses)
                .Where(c => request.CustomerIds.Contains(c.Id))
                .ToListAsync(cancellationToken);

            logger.LogDebug("Found {CustomerCount} customers", customers.Count);
            var result = mapper.Map<List<CustomerDto>>(customers);

            logger.LogDebug("{HandlerName} completed successfully. Returning {CustomerCount} customers", 
                nameof(GetCustomersByIdsQueryHandler), result.Count);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing {HandlerName} with customer IDs: {CustomerIds}", 
                nameof(GetCustomersByIdsQueryHandler), request.CustomerIds);
            throw;
        }
    }
}
