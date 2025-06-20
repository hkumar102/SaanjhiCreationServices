using MediatR;
using Shared.Contracts.Users;

namespace UserService.Application.Commands;

/// <summary>
/// Command to add a shipping address for a user.
/// </summary>
public class AddShippingAddressCommand : IRequest<ShippingAddressDto>
{
    public Guid UserId { get; set; }
    public string AddressLine1 { get; set; } = null!;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string Country { get; set; } = null!;
}
