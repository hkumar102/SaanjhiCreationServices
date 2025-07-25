using Shared.Domain.Entities;

namespace CustomerService.Domain.Entities;

public class Customer : BaseEntity
{
    public Guid? UserId { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
}