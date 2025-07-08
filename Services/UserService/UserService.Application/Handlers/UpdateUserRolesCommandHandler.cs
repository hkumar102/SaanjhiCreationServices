using Shared.Contracts.Users;
using UserService.Application.Commands;

namespace UserService.Application.Handlers;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Persistence;

public class UpdateUserRolesCommandHandler(UserDbContext dbContext, IMapper mapper)
    : IRequestHandler<UpdateUserRolesCommand, List<UserRoleDto>>
{
    public async Task<List<UserRoleDto>> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
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

        await dbContext.SaveChangesAsync(cancellationToken);

        return mapper.Map<List<UserRoleDto>>(user.Roles);
    }
}