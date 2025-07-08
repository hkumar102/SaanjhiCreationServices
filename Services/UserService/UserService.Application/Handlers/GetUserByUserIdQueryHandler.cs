using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Queries;
using UserService.Infrastructure.Persistence;
using Shared.Contracts.Users;

namespace UserService.Application.Handlers;

/// <summary>
/// Handles retrieval of user by UserId.
/// </summary>
public class GetUserByUserIdQueryHandler(UserDbContext context, IMapper mapper)
    : IRequestHandler<GetUserByUserIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByUserIdQuery request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Where(x => x.Id == request.UserId)
            .Select(x => mapper.Map<UserDto>(x)).FirstOrDefaultAsync(cancellationToken);

        return user ?? throw new KeyNotFoundException("User not found");
    }
}
