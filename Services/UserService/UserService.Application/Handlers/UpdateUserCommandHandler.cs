using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Commands;
using UserService.Infrastructure.Persistence;
using Shared.Contracts.Users;
using UserService.Domain.Entities;

namespace UserService.Application.Handlers;

/// <summary>
/// Handles user updates.
/// </summary>
public class UpdateUserCommandHandler(UserDbContext context, IMapper mapper)
    : IRequestHandler<UpdateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users.FindAsync(new object[] { request.Id }, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException("User not found");
    
        // Update fields
        user.DisplayName = request.DisplayName;
        user.PhoneNumber = request.PhoneNumber;
        user.PhotoUrl = request.PhotoUrl;
        user.IsActive = request.IsActive;
        user.EmailVerified = request.EmailVerified;
        
        // Replace roles
        user.Roles.Clear();
        foreach (var roleDto in request.Roles)
        {
            user.Roles.Add(new UserRole
            {
                RoleId = roleDto.Id
            });
        }
        
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<UserDto>(user);
    }
}
