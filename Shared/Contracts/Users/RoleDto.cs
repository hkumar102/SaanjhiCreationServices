using System;

namespace Shared.Contracts.Users;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
