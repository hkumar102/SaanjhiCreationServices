using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Commands;
using UserService.Infrastructure.Persistence;
using Shared.Contracts.Users;

namespace UserService.Application.Handlers;

/// <summary>
/// Handles user updates.
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly UserDbContext _context;

    public UpdateUserCommandHandler(UserDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        user.FullName = request.Name;
        user.PhoneNumber = request.Phone;

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
