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
