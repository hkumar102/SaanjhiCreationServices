using System;

namespace Shared.Contracts.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string FirebaseUserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? PhotoUrl { get; set; }
    public string Provider { get; set; } = null!;
    public bool IsActive { get; set; }
}
