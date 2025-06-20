namespace Shared.Authentication;

public interface ICurrentUserService
{
    string? FirebaseUserId { get; }
    string? UserId { get; }
    string? Email { get; }
    string? Name { get; }
}
