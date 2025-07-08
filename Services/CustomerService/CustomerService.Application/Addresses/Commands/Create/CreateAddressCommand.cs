using CustomerService.Contracts.Enums;
using CustomerService.Domain.Entities;
using MediatR;

namespace CustomerService.Application.Addresses.Commands.Create;

public class CreateAddressCommand : IRequest<Guid>
{
    public Guid CustomerId { get; set; }
    public string Line1 { get; set; } = default!;
    public string? Line2 { get; set; }
    public string City { get; set; } = default!;
    public string State { get; set; } = default!;
    public string PostalCode { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public AddressType Type { get; set; }
}