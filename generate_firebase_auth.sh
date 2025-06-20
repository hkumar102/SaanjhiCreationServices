#!/bin/bash

echo "🔐 Generating Firebase Authorization classes..."

BASE="./Shared/Authentication"
mkdir -p "$BASE"

# FirebaseAuthorizeAttribute.cs
cat > "$BASE/FirebaseAuthorizeAttribute.cs" <<EOF
using Microsoft.AspNetCore.Authorization;

namespace Shared.Authentication;

/// <summary>
/// Marks an endpoint as requiring Firebase authentication.
/// </summary>
public class FirebaseAuthorizeAttribute : AuthorizeAttribute
{
    public FirebaseAuthorizeAttribute()
    {
        Policy = FirebaseAuthorizationHandler.PolicyName;
    }
}
EOF

# FirebaseAuthorizationHandler.cs
cat > "$BASE/FirebaseAuthorizationHandler.cs" <<EOF
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Shared.Authentication;

/// <summary>
/// Authorization handler that validates Firebase JWT tokens.
/// </summary>
public class FirebaseAuthorizationHandler : AuthorizationHandler<AuthorizationRequirement>
{
    public const string PolicyName = "Firebase";

    private readonly FirebaseAuth _firebaseAuth;
    private readonly ILogger<FirebaseAuthorizationHandler> _logger;

    public FirebaseAuthorizationHandler(ILogger<FirebaseAuthorizationHandler> logger)
    {
        _firebaseAuth = FirebaseAuth.DefaultInstance;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AuthorizationRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
        {
            context.Fail();
            return;
        }

        var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            context.Fail();
            return;
        }

        var token = authHeader["Bearer ".Length..].Trim();

        try
        {
            var decodedToken = await _firebaseAuth.VerifyIdTokenAsync(token);
            var uid = decodedToken.Uid;

            var identity = new ClaimsIdentity("Firebase");
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, uid));
            identity.AddClaim(new Claim("firebase_user_id", uid));

            var principal = new ClaimsPrincipal(identity);
            context.User = principal;

            context.Succeed(requirement);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Firebase token verification failed.");
            context.Fail();
        }
    }
}
EOF

echo "✅ FirebaseAuthorizeAttribute and FirebaseAuthorizationHandler generated in $BASE"
