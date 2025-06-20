#!/bin/bash

echo "🔐 Creating ICurrentUserService and FirebaseCurrentUserService..."

dir="./Shared/Authentication"
mkdir -p "$dir"

# ICurrentUserService.cs
cat > "$dir/ICurrentUserService.cs" <<EOF
namespace Shared.Authentication;

public interface ICurrentUserService
{
    string? FirebaseUserId { get; }
    string? UserId { get; }
    string? Email { get; }
    string? Name { get; }
}
EOF

# FirebaseCurrentUserService.cs
cat > "$dir/FirebaseCurrentUserService.cs" <<EOF
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Shared.Authentication;

public class FirebaseCurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FirebaseCurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? FirebaseUserId =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? UserId => FirebaseUserId;

    public string? Email =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

    public string? Name =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
}
EOF

echo "✅ CurrentUserService generated in: $dir"
