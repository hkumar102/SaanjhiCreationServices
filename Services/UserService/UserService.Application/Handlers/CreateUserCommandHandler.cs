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
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly UserDbContext _context;
    private readonly IMapper _mapper;
    public CreateUserCommandHandler(UserDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = _mapper.Map<User>(request);
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<UserDto>(user);
    }
}
