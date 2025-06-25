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
public class GetUserByUserIdQueryHandler : IRequestHandler<GetUserByUserIdQuery, UserDto>
{
    private readonly UserDbContext _context;
    private readonly IMapper _mapper;
    public GetUserByUserIdQueryHandler(UserDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(GetUserByUserIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Where(x => x.Id == request.UserId)
            .Select(x => _mapper.Map<UserDto>(x)).FirstOrDefaultAsync(cancellationToken);

        return user ?? throw new KeyNotFoundException("User not found");
    }
}
