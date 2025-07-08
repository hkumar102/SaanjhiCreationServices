using RentalService.Contracts.DTOs;
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
    IProductApiClient productClient)
    : IRequestHandler<GetRentalByIdQuery, RentalDto>
{
    public async Task<RentalDto> Handle(GetRentalByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Rentals.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (entity == null) throw new KeyNotFoundException("Rental not found.");

        var dto = mapper.Map<RentalDto>(entity);
        dto.Customer = await customerClient.GetCustomerByIdAsync(entity.CustomerId, cancellationToken);
        var customerAddress =
            await customerClient.GetAddressByIdAsync(entity.ShippingAddressId, cancellationToken);
        dto.ShippingAddress = customerAddress?.Address();
        dto.Product = await productClient.GetProductByIdAsync(entity.ProductId, cancellationToken);
        
        return dto;
    }
}