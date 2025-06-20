using MediatR;
using Shared.Contracts.Users;

namespace UserService.Application.Commands;

/// <summary>
/// Command to update an existing user.
/// </summary>
public class UpdateUserCommand : IRequest<UserDto>
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
}
