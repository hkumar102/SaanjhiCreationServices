using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Common;
using UserService.Application.Queries;
using UserService.Infrastructure.Persistence;
using Shared.Contracts.Users;

namespace UserService.Application.Handlers;

/// <summary>
/// Handles user search with pagination and filters.
/// </summary>
public class SearchUsersQueryHandler(UserDbContext context, IMapper mapper)
    : IRequestHandler<SearchUsersQuery, PaginatedResult<UserDto>>
{
    public async Task<PaginatedResult<UserDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        var query = context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Name))
            query = query.Where(x => x.DisplayName.Contains(request.Name));

        if (!string.IsNullOrWhiteSpace(request.Email))
            query = query.Where(x => x.Email.Contains(request.Email));

        if (!string.IsNullOrWhiteSpace(request.Phone))
            query = query.Where(x => x.PhoneNumber!.Contains(request.Phone));

        var total = await query.CountAsync(cancellationToken);

        var users = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => mapper.Map<UserDto>(x)).ToListAsync(cancellationToken);

        return new PaginatedResult<UserDto>
        {
            TotalCount = total,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            Items = users
        };
    }
}
