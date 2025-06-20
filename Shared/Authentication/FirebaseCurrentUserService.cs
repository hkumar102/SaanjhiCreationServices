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
