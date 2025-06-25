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
public class AddShippingAddressCommandHandler : IRequestHandler<AddShippingAddressCommand, ShippingAddressDto>
{
    private readonly UserDbContext _context;
    private readonly IMapper _mapper;
    public AddShippingAddressCommandHandler(UserDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
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

        return _mapper.Map<ShippingAddressDto>(address);
    }
}
