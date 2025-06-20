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
