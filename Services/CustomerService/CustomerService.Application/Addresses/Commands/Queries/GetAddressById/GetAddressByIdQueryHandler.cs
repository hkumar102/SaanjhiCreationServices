using CustomerService.Domain.Entities;
using Microsoft.Extensions.Logging;
using CustomerService.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Application.Addresses.Commands.Queries.GetAddressById;

public class GetAddressByIdQueryHandler(
    CustomerDbContext context,
    ILogger<GetAddressByIdQueryHandler> logger) : IRequestHandler<GetAddressByIdQuery, Address?>
{
    public async Task<Address?> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting {HandlerName} execution for address ID: {AddressId}", 
            nameof(GetAddressByIdQueryHandler), request.Id);

        try
        {
            logger.LogDebug("Fetching address with ID: {AddressId} from database", request.Id);
            var address = await context.Addresses
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (address != null)
            {
                logger.LogDebug("Found address: {AddressDetails}", new { address.Line1, address.City, address.CustomerId });
            }
            else
            {
                logger.LogDebug("Address not found with ID: {AddressId}", request.Id);
            }

            logger.LogDebug("{HandlerName} completed successfully for address ID: {AddressId}", 
                nameof(GetAddressByIdQueryHandler), request.Id);

            return address;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing {HandlerName} for address ID: {AddressId}", 
                nameof(GetAddressByIdQueryHandler), request.Id);
            throw;
        }
    }
}
