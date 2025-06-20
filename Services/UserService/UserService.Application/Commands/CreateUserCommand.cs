using MediatR;
using Shared.Contracts.Users;

namespace UserService.Application.Commands;

/// <summary>
/// Command to create a new user.
/// </summary>
public class CreateUserCommand : IRequest<UserDto>
{
    public string FirebaseUserId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
}
