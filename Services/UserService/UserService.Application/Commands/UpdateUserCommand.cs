using MediatR;
using Shared.Contracts.Users;

namespace UserService.Application.Commands;

/// <summary>
/// Command to update an existing user.
/// </summary>
public class UpdateUserCommand : IRequest<UserDto>
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? PhotoUrl { get; set; }
    public bool IsActive { get; set; }
    public bool EmailVerified { get; set; }
    public List<UserRoleDto> Roles { get; set; } = new();
}
