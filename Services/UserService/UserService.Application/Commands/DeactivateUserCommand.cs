using MediatR;

namespace UserService.Application.Commands;

/// <summary>
/// Command to deactivate a user.
/// </summary>
public class DeactivateUserCommand : IRequest
{
    public Guid UserId { get; set; }
}
