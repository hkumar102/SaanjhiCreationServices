using MediatR;

namespace UserService.Application.Queries;

/// <summary>
/// Query to get the roles assigned to a user.
/// </summary>
public class GetUserRolesQuery : IRequest<List<string>>
{
    public Guid UserId { get; set; }
}
