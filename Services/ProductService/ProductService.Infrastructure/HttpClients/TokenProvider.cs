using Microsoft.AspNetCore.Http;

namespace ProductService.Infrastructure.HttpClients;

public class TokenProvider(IHttpContextAccessor httpContextAccessor) : ITokenProvider
{
    public Task<string> GetTokenAsync()
    {
        var authHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            return Task.FromResult(authHeader.Substring("Bearer ".Length));
        }
        return Task.FromResult(string.Empty);
    }
}