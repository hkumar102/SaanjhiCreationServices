using MediatR;
using UserService.Application.Commands;
using UserService.Infrastructure.Persistence;
using UserService.Domain.Entities;
using Shared.Contracts.Users;

namespace UserService.Application.Handlers;

/// <summary>
/// Handles user creation.
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly UserDbContext _context;

    public CreateUserCommandHandler(UserDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            FirebaseUserId = request.FirebaseUserId,
            FullName = request.Name,
            Email = request.Email,
            PhoneNumber = request.Phone
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };
    }
}
