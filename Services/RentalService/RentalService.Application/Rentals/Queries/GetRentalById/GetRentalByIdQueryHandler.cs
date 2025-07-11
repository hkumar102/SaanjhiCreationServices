using RentalService.Contracts.DTOs;
using Microsoft.Extensions.Logging;
using RentalService.Infrastructure.HttpClients;

namespace RentalService.Application.Rentals.Queries.GetRentalById;

using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;

public class GetRentalByIdQueryHandler(
    RentalDbContext dbContext,
    IMapper mapper,
    ICustomerApiClient customerClient,
    IProductApiClient productClient,
    ILogger<GetRentalByIdQueryHandler> logger)
    : IRequestHandler<GetRentalByIdQuery, RentalDto>
{
    public async Task<RentalDto> Handle(GetRentalByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting {HandlerName} execution for rental ID: {RentalId}", 
            nameof(GetRentalByIdQueryHandler), request.Id);

        try
        {
            logger.LogDebug("Fetching rental with ID: {RentalId} from database", request.Id);
            var entity = await dbContext.Rentals.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                logger.LogDebug("Rental not found with ID: {RentalId}", request.Id);
                throw new KeyNotFoundException("Rental not found.");
            }

            logger.LogDebug("Found rental: CustomerId={CustomerId}, ProductId={ProductId}", 
                entity.CustomerId, entity.ProductId);

            var dto = mapper.Map<RentalDto>(entity);
            
            logger.LogDebug("Fetching customer data for CustomerId: {CustomerId}", entity.CustomerId);
            dto.Customer = await customerClient.GetCustomerByIdAsync(entity.CustomerId, cancellationToken);
            
            logger.LogDebug("Fetching shipping address for AddressId: {AddressId}", entity.ShippingAddressId);
            var customerAddress = await customerClient.GetAddressByIdAsync(entity.ShippingAddressId, cancellationToken);
            dto.ShippingAddress = customerAddress?.Address();
            
            logger.LogDebug("Fetching product data for ProductId: {ProductId}", entity.ProductId);
            dto.Product = await productClient.GetProductByIdAsync(entity.ProductId, cancellationToken);

            logger.LogDebug("{HandlerName} completed successfully for rental ID: {RentalId}", 
                nameof(GetRentalByIdQueryHandler), request.Id);

            return dto;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing {HandlerName} for rental ID: {RentalId}", 
                nameof(GetRentalByIdQueryHandler), request.Id);
            throw;
        }
    }
}
