using MediatR;
using Shared.Contracts.Users;

namespace UserService.Application.Commands;

/// <summary>
/// Command to create a new user.
/// </summary>
public class CreateUserCommand : IRequest<UserDto>
{
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? PhotoUrl { get; set; }
    public bool IsActive { get; set; }
    public bool EmailVerified { get; set; }
    public string FirebaseUserId { get; set; } = null!;
    public List<AuthProviderDto> providers { get; set; } = new();
}
