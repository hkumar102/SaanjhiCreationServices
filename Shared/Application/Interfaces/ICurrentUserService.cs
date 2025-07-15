using System.Collections.Generic;

namespace Shared.Application.Interfaces;

/// <summary>
/// Service to get current user information from Firebase authentication
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's Firebase UID
    /// </summary>
    string? UserId { get; }
    
    /// <summary>
    /// Gets the current user's display name
    /// </summary>
    string? UserName { get; }
    
    /// <summary>
    /// Gets the current user's email address
    /// </summary>
    string? Email { get; }
    
    /// <summary>
    /// Gets the Firebase sign-in provider (google.com, password, etc.)
    /// </summary>
    string? SignInProvider { get; }
    
    /// <summary>
    /// Checks if the user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }
    
    /// <summary>
    /// Gets user roles from Firebase custom claims
    /// </summary>
    IEnumerable<string> Roles { get; }
    
    /// <summary>
    /// Checks if user has a specific role
    /// </summary>
    /// <param name="role">Role to check</param>
    /// <returns>True if user has the role</returns>
    bool HasRole(string role);
    
    /// <summary>
    /// Gets a custom claim value from Firebase
    /// </summary>
    /// <param name="claimType">Custom claim type</param>
    /// <returns>Claim value or null</returns>
    string? GetCustomClaim(string claimType);
}
