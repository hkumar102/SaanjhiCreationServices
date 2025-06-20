using MediatR;
using UserService.Application.Commands;
using UserService.Infrastructure.Persistence;

namespace UserService.Application.Handlers;

/// <summary>
/// Handles user deactivation.
/// </summary>
public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand>
{
    private readonly UserDbContext _context;

    public DeactivateUserCommandHandler(UserDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        user.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
