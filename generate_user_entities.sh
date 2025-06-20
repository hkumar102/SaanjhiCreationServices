#!/bin/bash

echo "📦 Creating UserService entities..."

entity_dir="./services/UserService/UserService.Domain/Entities"
mkdir -p "$entity_dir"

# User.cs
cat > "$entity_dir/User.cs" <<EOF
using Shared.Domain.Entities;

namespace UserService.Domain.Entities;

public class User : AuditableEntity
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? PhotoUrl { get; set; }

    public string Provider { get; set; } = null!;

    public ICollection<UserRole> Roles { get; set; } = new List<UserRole>();

    public ICollection<ShippingAddress> ShippingAddresses { get; set; } = new List<ShippingAddress>();
}
EOF

# Role.cs
cat > "$entity_dir/Role.cs" <<EOF
namespace UserService.Domain.Entities;

public class Role
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
EOF

# UserRole.cs
cat > "$entity_dir/UserRole.cs" <<EOF
namespace UserService.Domain.Entities;

public class UserRole
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
}
EOF

# ShippingAddress.cs
cat > "$entity_dir/ShippingAddress.cs" <<EOF
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
EOF

echo "✅ UserService entities created in: $entity_dir"
