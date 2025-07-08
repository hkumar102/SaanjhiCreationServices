using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Shared.Authentication;

public class FirebaseCurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? FirebaseUserId =>
        httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? UserId => FirebaseUserId;

    public string? Email =>
        httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

    public string? Name =>
        httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
}
