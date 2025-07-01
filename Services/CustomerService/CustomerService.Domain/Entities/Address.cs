using Shared.Domain.Entities;

namespace CustomerService.Domain.Entities;

public class Address : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string Line1 { get; set; } = default!;
    public string? Line2 { get; set; }
    public string City { get; set; } = default!;
    public string State { get; set; } = default!;
    public string PostalCode { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public AddressType Type { get; set; }
    public Customer Customer { get; set; } = default!;
}

public enum AddressType
{
    Shipping,
    Mailing
}
