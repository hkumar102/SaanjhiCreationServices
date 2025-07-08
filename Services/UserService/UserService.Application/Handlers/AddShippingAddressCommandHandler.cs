using AutoMapper;
using MediatR;
using UserService.Application.Commands;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;
using Shared.Contracts.Users;

namespace UserService.Application.Handlers;

/// <summary>
/// Handles adding a shipping address.
/// </summary>
public class AddShippingAddressCommandHandler(UserDbContext context, IMapper mapper)
    : IRequestHandler<AddShippingAddressCommand, ShippingAddressDto>
{
    public async Task<ShippingAddressDto> Handle(AddShippingAddressCommand request, CancellationToken cancellationToken)
    {
        var address = new ShippingAddress
        {
            UserId = request.UserId,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            ZipCode = request.PostalCode,
            Country = request.Country
        };

        context.ShippingAddresses.Add(address);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<ShippingAddressDto>(address);
    }
}
