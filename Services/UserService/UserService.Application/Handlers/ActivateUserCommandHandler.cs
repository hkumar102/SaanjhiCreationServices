using MediatR;
using UserService.Infrastructure.Persistence;
using UserService.Application.Commands;

namespace UserService.Application.Handlers;

/// <summary>
/// Handles user activation.
/// </summary>
public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand>
{
    private readonly UserDbContext _context;

    public ActivateUserCommandHandler(UserDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        user.IsActive = true;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
