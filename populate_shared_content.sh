#!/bin/bash

echo "📁 Creating Shared folder structure..."

mkdir -p Shared/Domain/Entities
mkdir -p Shared/Authentication
mkdir -p Shared/Contracts/Users

echo "📝 Adding AuditableEntity.cs..."
cat > Shared/Domain/Entities/AuditableEntity.cs <<EOF
namespace Shared.Domain.Entities;

public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}
EOF

echo "📝 Adding FirebaseAuthorizeAttribute.cs..."
cat > Shared/Authentication/FirebaseAuthorizeAttribute.cs <<EOF
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace Shared.Authentication;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class FirebaseAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

        if (authHeader == null || !authHeader.StartsWith("Bearer "))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();

        var handler = new JwtSecurityTokenHandler();

        try
        {
            var jwt = handler.ReadJwtToken(token);
            var uid = jwt.Subject;

            if (string.IsNullOrEmpty(uid))
            {
                context.Result = new UnauthorizedResult();
            }
        }
        catch
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
EOF

echo "📝 Adding UserDto.cs..."
cat > Shared/Contracts/Users/UserDto.cs <<EOF
namespace Shared.Contracts.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? PhotoUrl { get; set; }
    public string Provider { get; set; } = null!;
}
EOF

echo "📝 Adding RoleDto.cs..."
cat > Shared/Contracts/Users/RoleDto.cs <<EOF
namespace Shared.Contracts.Users;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
EOF

echo "📝 Adding ShippingAddressDto.cs..."
cat > Shared/Contracts/Users/ShippingAddressDto.cs <<EOF
namespace Shared.Contracts.Users;

public class ShippingAddressDto
{
    public Guid Id { get; set; }
    public string AddressLine1 { get; set; } = null!;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string ZipCode { get; set; } = null!;
    public string Country { get; set; } = null!;
}
EOF

echo "✅ Shared content populated successfully."
