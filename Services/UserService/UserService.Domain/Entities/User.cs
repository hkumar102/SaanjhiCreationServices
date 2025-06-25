using Shared.Domain.Entities;

namespace UserService.Domain.Entities;

public class User : AuditableEntity
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? PhotoUrl { get; set; }
    public bool IsActive { get; set; }
    public bool EmailVerified { get; set; }
    public string FirebaseUserId { get; set; }
    public ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
    public ICollection<ShippingAddress> ShippingAddresses { get; set; } = new List<ShippingAddress>();
    public ICollection<AuthProvider> Providers { get; set; } = new List<AuthProvider>();
}
