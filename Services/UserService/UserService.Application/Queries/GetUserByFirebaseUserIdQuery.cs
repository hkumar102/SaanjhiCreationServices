using MediatR;
using Shared.Contracts.Users;

namespace UserService.Application.Queries;

/// <summary>
/// Query to retrieve a user by Firebase UID.
/// </summary>
public class GetUserByFirebaseUserIdQuery : IRequest<UserDto>
{
    public string FirebaseUserId { get; set; } = null!;
}
