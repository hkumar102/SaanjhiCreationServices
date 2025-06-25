using Shared.Authentication;
using UserService.Application.Commands;

namespace UserService.Application.Handlers;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand>
{
    private readonly UserDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateUserProfileCommandHandler(UserDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.FirebaseUserId == _currentUser.FirebaseUserId, cancellationToken);

        if (user == null)
            throw new KeyNotFoundException("User not found");

        user.DisplayName = request.DisplayName;
        user.PhoneNumber = request.PhoneNumber;
        user.PhotoUrl = request.PhotoUrl;

        await _db.SaveChangesAsync(cancellationToken);
    }
}