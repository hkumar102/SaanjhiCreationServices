using Shared.Authentication;
using UserService.Application.Commands;

namespace UserService.Application.Handlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;

public class UpdateUserProfileCommandHandler(UserDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<UpdateUserProfileCommand>
{
    public async Task Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.FirebaseUserId == currentUser.FirebaseUserId, cancellationToken);

        if (user == null)
            throw new KeyNotFoundException("User not found");

        user.DisplayName = request.DisplayName;
        user.PhoneNumber = request.PhoneNumber;
        user.PhotoUrl = request.PhotoUrl;

        await db.SaveChangesAsync(cancellationToken);
    }
}