namespace UserService.Domain.Entities;

// Domain/Entities/AuthProvider.cs
public class AuthProvider
{
    public int Id { get; set; } // Primary Key
    public string ProviderId { get; set; } = null!;   // e.g. "google.com"
    public string Uid { get; set; } = null!;          // UID from the provider
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public string? PhotoUrl { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
