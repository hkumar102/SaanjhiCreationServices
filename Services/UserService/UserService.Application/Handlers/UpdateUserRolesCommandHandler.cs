using Shared.Contracts.Users;
using UserService.Application.Commands;

namespace UserService.Application.Handlers;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Persistence;

public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, List<UserRoleDto>>
{
    private readonly UserDbContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateUserRolesCommandHandler(UserDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<UserRoleDto>> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
            throw new KeyNotFoundException($"User with ID {request.UserId} not found.");

        // Clear and replace roles
        user.Roles.Clear();

        foreach (var dto in request.Roles)
        {
            user.Roles.Add(new UserRole
            {
                RoleId = dto.Id,
                UserId = request.UserId
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<List<UserRoleDto>>(user.Roles);
    }
}