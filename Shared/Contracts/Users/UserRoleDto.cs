using System;

namespace Shared.Contracts.Users;

public class UserRoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}