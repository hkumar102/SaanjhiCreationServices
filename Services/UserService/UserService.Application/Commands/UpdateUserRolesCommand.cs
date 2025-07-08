using Shared.Contracts.Users;

namespace UserService.Application.Commands;

using MediatR;

public class UpdateUserRolesCommand : IRequest<List<UserRoleDto>>
{
    public Guid UserId { get; set; }
    public List<UserRoleDto> Roles { get; set; } = new();
}