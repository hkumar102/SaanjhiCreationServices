using Shared.Domain.Entities;

namespace UserService.Domain.Entities;

public class ShippingAddress : AuditableEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string AddressLine1 { get; set; } = null!;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string ZipCode { get; set; } = null!;
    public string Country { get; set; } = null!;
}
