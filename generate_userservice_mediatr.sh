#!/bin/bash

echo "🚀 Generating MediatR structure for UserService..."

BASE="./services/UserService/UserService.Application"

mkdir -p \
  "$BASE/Commands" \
  "$BASE/Queries" \
  "$BASE/Handlers" \
  "$BASE/Validators"

# ----------- COMMANDS -----------

cat > "$BASE/Commands/CreateUserCommand.cs" <<EOF
using MediatR;
using Shared.Contracts.Users;

namespace UserService.Application.Commands;

/// <summary>
/// Command to create a new user.
/// </summary>
public class CreateUserCommand : IRequest<UserDto>
{
    public string FirebaseUserId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
}
EOF

cat > "$BASE/Commands/UpdateUserCommand.cs" <<EOF
using MediatR;
using Shared.Contracts.Users;

namespace UserService.Application.Commands;

/// <summary>
/// Command to update an existing user.
/// </summary>
public class UpdateUserCommand : IRequest<UserDto>
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
}
EOF

cat > "$BASE/Commands/AddShippingAddressCommand.cs" <<EOF
using MediatR;
using Shared.Contracts.Users;

namespace UserService.Application.Commands;

/// <summary>
/// Command to add a shipping address for a user.
/// </summary>
public class AddShippingAddressCommand : IRequest<ShippingAddressDto>
{
    public Guid UserId { get; set; }
    public string AddressLine1 { get; set; } = null!;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string Country { get; set; } = null!;
}
EOF

cat > "$BASE/Commands/DeactivateUserCommand.cs" <<EOF
using MediatR;

namespace UserService.Application.Commands;

/// <summary>
/// Command to deactivate a user.
/// </summary>
public class DeactivateUserCommand : IRequest
{
    public Guid UserId { get; set; }
}
EOF

cat > "$BASE/Commands/ActivateUserCommand.cs" <<EOF
using MediatR;

namespace UserService.Application.Commands;

/// <summary>
/// Command to activate a user.
/// </summary>
public class ActivateUserCommand : IRequest
{
    public Guid UserId { get; set; }
}
EOF

# ----------- QUERIES -----------

cat > "$BASE/Queries/GetUserByFirebaseUserIdQuery.cs" <<EOF
using MediatR;
using Shared.Contracts.Users;

namespace UserService.Application.Queries;

/// <summary>
/// Query to retrieve a user by Firebase UID.
/// </summary>
public class GetUserByFirebaseUserIdQuery : IRequest<UserDto>
{
    public string FirebaseUserId { get; set; } = null!;
}
EOF

cat > "$BASE/Queries/GetUserByUserIdQuery.cs" <<EOF
using MediatR;
using Shared.Contracts.Users;

namespace UserService.Application.Queries;

/// <summary>
/// Query to retrieve a user by UserId.
/// </summary>
public class GetUserByUserIdQuery : IRequest<UserDto>
{
    public Guid UserId { get; set; }
}
EOF

cat > "$BASE/Queries/GetUserRolesQuery.cs" <<EOF
using MediatR;

namespace UserService.Application.Queries;

/// <summary>
/// Query to get the roles assigned to a user.
/// </summary>
public class GetUserRolesQuery : IRequest<List<string>>
{
    public Guid UserId { get; set; }
}
EOF

cat > "$BASE/Queries/SearchUsersQuery.cs" <<EOF
using MediatR;
using Shared.Contracts.Users;

namespace UserService.Application.Queries;

/// <summary>
/// Query to search users with pagination and filters.
/// </summary>
public class SearchUsersQuery : IRequest<PaginatedResult<UserDto>>
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
EOF

echo "✅ UserService MediatR command and query definitions generated."
