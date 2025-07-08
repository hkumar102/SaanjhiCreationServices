using MediatR;
using UserService.Application.Commands;
using UserService.Infrastructure.Persistence;

namespace UserService.Application.Handlers;

/// <summary>
/// Handles user deactivation.
/// </summary>
public class DeactivateUserCommandHandler(UserDbContext context) : IRequestHandler<DeactivateUserCommand>
{
    public async Task Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        user.IsActive = false;
        await context.SaveChangesAsync(cancellationToken);
    }
}
