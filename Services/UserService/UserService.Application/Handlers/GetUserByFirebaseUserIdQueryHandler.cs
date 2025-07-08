using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Queries;
using UserService.Infrastructure.Persistence;
using Shared.Contracts.Users;

namespace UserService.Application.Handlers;

/// <summary>
/// Handles retrieval of user by FirebaseUserId.
/// </summary>
public class GetUserByFirebaseUserIdQueryHandler(UserDbContext context, IMapper mapper)
    : IRequestHandler<GetUserByFirebaseUserIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByFirebaseUserIdQuery request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Where(x => x.FirebaseUserId == request.FirebaseUserId)
            .Select(x =>  mapper.Map<UserDto>(x)).FirstOrDefaultAsync(cancellationToken);

        return user ?? throw new KeyNotFoundException("User not found");
    }
}
