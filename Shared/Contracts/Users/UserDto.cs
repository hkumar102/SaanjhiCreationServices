using System;
using System.Collections.Generic;

namespace Shared.Contracts.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? PhotoUrl { get; set; }
    public bool IsActive { get; set; }
    public bool EmailVerified { get; set; }
    public string FirebaseUserId { get; set; }

    public List<UserRoleDto> Roles { get; set; } = new();
    public List<ShippingAddressDto> ShippingAddresses { get; set; } = new();
    public List<AuthProviderDto> AuthProviders { get; set; } = new();
}
