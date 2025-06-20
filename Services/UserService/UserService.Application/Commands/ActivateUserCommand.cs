using MediatR;

namespace UserService.Application.Commands;

/// <summary>
/// Command to activate a user.
/// </summary>
public class ActivateUserCommand : IRequest
{
    public Guid UserId { get; set; }
}
