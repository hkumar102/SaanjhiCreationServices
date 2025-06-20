using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Queries;
using UserService.Infrastructure.Persistence;
using Shared.Contracts.Users;

namespace UserService.Application.Handlers;

/// <summary>
/// Handles retrieval of user by UserId.
/// </summary>
public class GetUserByUserIdQueryHandler : IRequestHandler<GetUserByUserIdQuery, UserDto>
{
    private readonly UserDbContext _context;

    public GetUserByUserIdQueryHandler(UserDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(GetUserByUserIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Where(x => x.Id == request.UserId)
            .Select(x => new UserDto
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber
            }).FirstOrDefaultAsync(cancellationToken);

        return user ?? throw new KeyNotFoundException("User not found");
    }
}
