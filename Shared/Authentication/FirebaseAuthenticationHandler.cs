using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Shared.Authentication;

/// <summary>
/// Firebase authentication handler that validates bearer tokens.
/// </summary>
public class FirebaseAuthenticationHandler(
    IOptionsMonitor<FirebaseAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<FirebaseAuthenticationOptions>(options, logger, encoder)
{
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
                // Standard claims
                new(ClaimTypes.NameIdentifier, decoded.Uid),
                new("sub", decoded.Uid), // Standard JWT sub claim
                new("uid", decoded.Uid), // Firebase-specific uid
                new("firebase_user_id", decoded.Uid), // Legacy support
            };

            // Add email if available
            if (!string.IsNullOrEmpty(decoded.Claims.GetValueOrDefault("email")?.ToString()))
            {
                var email = decoded.Claims["email"].ToString();
                claims.Add(new Claim(ClaimTypes.Email, email!));
                claims.Add(new Claim("email", email!));
            }

            // Add name if available
            if (!string.IsNullOrEmpty(decoded.Claims.GetValueOrDefault("name")?.ToString()))
            {
                var name = decoded.Claims["name"].ToString();
                claims.Add(new Claim(ClaimTypes.Name, name!));
                claims.Add(new Claim("name", name!));
            }

            // Add display name if available (Firebase specific)
            if (!string.IsNullOrEmpty(decoded.Claims.GetValueOrDefault("firebase")?.ToString()))
            {
                var firebaseData = decoded.Claims["firebase"] as System.Collections.Generic.Dictionary<string, object>;
                if (firebaseData?.TryGetValue("sign_in_provider", out var provider) == true)
                {
                    claims.Add(new Claim("sign_in_provider", provider.ToString()!));
                }
            }

            // Add custom claims if any
            if (decoded.Claims.TryGetValue("custom_claims", out var customClaims) && customClaims != null)
            {
                var customClaimsDict = customClaims as System.Collections.Generic.Dictionary<string, object>;
                if (customClaimsDict != null)
                {
                    foreach (var claim in customClaimsDict)
                    {
                        claims.Add(new Claim($"custom_{claim.Key}", claim.Value?.ToString() ?? ""));
                    }
                }
            }

            // Add roles if available in custom claims
            if (decoded.Claims.TryGetValue("role", out var role) && role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.ToString()!));
            }

            if (decoded.Claims.TryGetValue("roles", out var roles) && roles != null)
            {
                if (roles is System.Collections.Generic.List<object> rolesList)
                {
                    foreach (var r in rolesList)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, r.ToString()!));
                    }
                }
            }

            // Add issuer and audience for additional verification
            claims.Add(new Claim("iss", decoded.Issuer));
            claims.Add(new Claim("aud", decoded.Audience));

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
