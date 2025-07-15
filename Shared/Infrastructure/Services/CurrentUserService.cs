using Microsoft.AspNetCore.Http;
using Shared.Application.Interfaces;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Infrastructure.Services;

/// <summary>
/// Implementation of ICurrentUserService using HTTP context with Firebase authentication
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue("uid") ??           // Firebase uid
        _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub") ??           // Standard JWT sub
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? // Standard claim
        _httpContextAccessor.HttpContext?.User?.FindFirstValue("firebase_user_id"); // Legacy Firebase

    public string? UserName =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name) ??  // Standard name claim
        _httpContextAccessor.HttpContext?.User?.FindFirstValue("name") ??           // JWT name claim
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email) ?? // Fallback to email
        _httpContextAccessor.HttpContext?.User?.FindFirstValue("email");            // JWT email claim

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    /// <summary>
    /// Gets the user's email address
    /// </summary>
    public string? Email =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email) ??
        _httpContextAccessor.HttpContext?.User?.FindFirstValue("email");

    /// <summary>
    /// Gets the Firebase sign-in provider (google.com, password, etc.)
    /// </summary>
    public string? SignInProvider =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue("sign_in_provider");

    /// <summary>
    /// Gets user roles from Firebase custom claims
    /// </summary>
    public IEnumerable<string> Roles =>
        _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role)?.Select(c => c.Value) ?? 
        Enumerable.Empty<string>();

    /// <summary>
    /// Checks if user has a specific role
    /// </summary>
    /// <param name="role">Role to check</param>
    /// <returns>True if user has the role</returns>
    public bool HasRole(string role) =>
        _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;

    /// <summary>
    /// Gets a custom claim value from Firebase
    /// </summary>
    /// <param name="claimType">Custom claim type (will be prefixed with 'custom_')</param>
    /// <returns>Claim value or null</returns>
    public string? GetCustomClaim(string claimType) =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue($"custom_{claimType}");
}
