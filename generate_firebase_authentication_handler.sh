#!/bin/bash

echo "🔐 Generating Firebase Authentication Handler and supporting files..."

BASE="./Shared/Authentication"
mkdir -p "$BASE"

# FirebaseAuthenticationDefaults.cs
cat > "$BASE/FirebaseAuthenticationDefaults.cs" <<EOF
namespace Shared.Authentication;

/// <summary>
/// Default scheme name for Firebase authentication.
/// </summary>
public static class FirebaseAuthenticationDefaults
{
    public const string Scheme = "Firebase";
}
EOF

# FirebaseAuthenticationOptions.cs
cat > "$BASE/FirebaseAuthenticationOptions.cs" <<EOF
using Microsoft.AspNetCore.Authentication;

namespace Shared.Authentication;

/// <summary>
/// Firebase authentication options.
/// </summary>
public class FirebaseAuthenticationOptions : AuthenticationSchemeOptions
{
}
EOF

# FirebaseAuthenticationHandler.cs
cat > "$BASE/FirebaseAuthenticationHandler.cs" <<EOF
using System.Security.Claims;
using System.Text.Encodings.Web;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Shared.Authentication;

/// <summary>
/// Firebase authentication handler that validates bearer tokens.
/// </summary>
public class FirebaseAuthenticationHandler : AuthenticationHandler<FirebaseAuthenticationOptions>
{
    public FirebaseAuthenticationHandler(
        IOptionsMonitor<FirebaseAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock) { }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
            return AuthenticateResult.NoResult();

        string token = Request.Headers["Authorization"]
            .ToString()
            .Replace("Bearer ", string.Empty);

        if (string.IsNullOrWhiteSpace(token))
            return AuthenticateResult.Fail("Missing token");

        try
        {
            var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, decoded.Uid),
                new("firebase_user_id", decoded.Uid),
            };

            var identity = new ClaimsIdentity(claims, FirebaseAuthenticationDefaults.Scheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, FirebaseAuthenticationDefaults.Scheme);

            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Firebase authentication failed.");
            return AuthenticateResult.Fail("Invalid Firebase token");
        }
    }
}
EOF

# AuthenticationExtensions.cs
cat > "$BASE/AuthenticationExtensions.cs" <<EOF
using Microsoft.AspNetCore.Authentication;

namespace Shared.Authentication;

/// <summary>
/// Extension method to register Firebase authentication.
/// </summary>
public static class AuthenticationExtensions
{
    public static AuthenticationBuilder AddFirebaseAuthentication(this AuthenticationBuilder builder)
    {
        return builder.AddScheme<FirebaseAuthenticationOptions, FirebaseAuthenticationHandler>(
            FirebaseAuthenticationDefaults.Scheme, options => { });
    }
}
EOF

echo "✅ Firebase AuthenticationHandler generated successfully in $BASE"
