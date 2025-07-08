using MediatR;
using UserService.Infrastructure.Persistence;
using UserService.Application.Commands;

namespace UserService.Application.Handlers;

/// <summary>
/// Handles user activation.
/// </summary>
public class ActivateUserCommandHandler(UserDbContext context) : IRequestHandler<ActivateUserCommand>
{
    public async Task Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        user.IsActive = true;
        await context.SaveChangesAsync(cancellationToken);
    }
}
