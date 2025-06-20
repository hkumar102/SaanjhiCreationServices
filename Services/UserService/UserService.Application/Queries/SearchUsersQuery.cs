using MediatR;
using Shared.Contracts.Common;
using Shared.Contracts.Users;

namespace UserService.Application.Queries;

/// <summary>
/// Query to search users with pagination and filters.
/// </summary>
public class SearchUsersQuery : IRequest<PaginatedResult<UserDto>>
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
