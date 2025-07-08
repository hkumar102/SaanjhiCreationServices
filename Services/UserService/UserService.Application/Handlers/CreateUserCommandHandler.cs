using AutoMapper;
using MediatR;
using UserService.Application.Commands;
using UserService.Infrastructure.Persistence;
using UserService.Domain.Entities;
using Shared.Contracts.Users;

namespace UserService.Application.Handlers;

/// <summary>
/// Handles user creation.
/// </summary>
public class CreateUserCommandHandler(UserDbContext context, IMapper mapper)
    : IRequestHandler<CreateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = mapper.Map<User>(request);
        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);
        return mapper.Map<UserDto>(user);
    }
}
