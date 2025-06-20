using MediatR;
using Shared.Contracts.Users;

namespace UserService.Application.Queries;

/// <summary>
/// Query to retrieve a user by UserId.
/// </summary>
public class GetUserByUserIdQuery : IRequest<UserDto>
{
    public Guid UserId { get; set; }
}
