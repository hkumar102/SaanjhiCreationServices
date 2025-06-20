using MediatR;
using UserService.Application.Commands;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;
using Shared.Contracts.Users;

namespace UserService.Application.Handlers;

/// <summary>
/// Handles adding a shipping address.
/// </summary>
public class AddShippingAddressCommandHandler : IRequestHandler<AddShippingAddressCommand, ShippingAddressDto>
{
    private readonly UserDbContext _context;

    public AddShippingAddressCommandHandler(UserDbContext context)
    {
        _context = context;
    }

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

        _context.ShippingAddresses.Add(address);
        await _context.SaveChangesAsync(cancellationToken);

        return new ShippingAddressDto
        {
            Id = address.Id,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            City = address.City,
            State = address.State,
            ZipCode = address.ZipCode,
            Country = address.Country
        };
    }
}
