namespace Shared.Contracts.Users;

public class AuthProviderDto
{
    public string ProviderId { get; set; } = null!;
    public string Uid { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public string? PhotoUrl { get; set; }
}