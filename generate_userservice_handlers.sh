#!/bin/bash

echo "📦 Generating Handler implementations for UserService..."

BASE="./services/UserService/UserService.Application/Handlers"
mkdir -p "$BASE"

# CreateUserCommandHandler.cs
cat > "$BASE/CreateUserCommandHandler.cs" <<EOF
using MediatR;
using UserService.Application.Commands;
using UserService.Infrastructure.Persistence;
using UserService.Domain.Entities;
using Shared.Contracts.Users;

namespace UserService.Application.Handlers;

/// <summary>
/// Handles user creation.
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly UserDbContext _context;

    public CreateUserCommandHandler(UserDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            FirebaseUserId = request.FirebaseUserId,
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone
        };
    }
}
EOF

# UpdateUserCommandHandler.cs
cat > "$BASE/UpdateUserCommandHandler.cs" <<EOF
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Commands;
using UserService.Infrastructure.Persistence;
using Shared.Contracts.Users;

namespace UserService.Application.Handlers;

/// <summary>
/// Handles user updates.
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly UserDbContext _context;

    public UpdateUserCommandHandler(UserDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        user.Name = request.Name;
        user.Phone = request.Phone;

        await _context.SaveChangesAsync(cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone
        };
    }
}
EOF

# AddShippingAddressCommandHandler.cs
cat > "$BASE/AddShippingAddressCommandHandler.cs" <<EOF
using MediatR;
using UserService.Application.Commands;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;
using Shared.Contracts.Users;

namespace UserService.Application.Handlers;

/// <summary>
/// Handles adding a shipping address.
/// </summary>
public class AddShippingAddressCommandHandler : IRequestHandler<AddShippingAddressCommand, ShippingAddressDto>
{
    private readonly UserDbContext _context;

    public AddShippingAddressCommandHandler(UserDbContext context)
    {
        _context = context;
    }

    public async Task<ShippingAddressDto> Handle(AddShippingAddressCommand request, CancellationToken cancellationToken)
    {
        var address = new ShippingAddress
        {
            UserId = request.UserId,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Country = request.Country
        };

        _context.ShippingAddresses.Add(address);
        await _context.SaveChangesAsync(cancellationToken);

        return new ShippingAddressDto
        {
            Id = address.Id,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country
        };
    }
}
EOF

# DeactivateUserCommandHandler.cs
cat > "$BASE/DeactivateUserCommandHandler.cs" <<EOF
using MediatR;
using UserService.Application.Commands;
using UserService.Infrastructure.Persistence;

namespace UserService.Application.Handlers;

/// <summary>
/// Handles user deactivation.
/// </summary>
public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand>
{
    private readonly UserDbContext _context;

    public DeactivateUserCommandHandler(UserDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        user.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
EOF

# ActivateUserCommandHandler.cs
cat > "$BASE/ActivateUserCommandHandler.cs" <<EOF
using MediatR;
using UserService.Application.Commands;
using UserService.Infrastructure.Persistence;

namespace UserService.Application.Handlers;

/// <summary>
/// Handles user activation.
/// </summary>
public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand>
{
    private readonly UserDbContext _context;

    public ActivateUserCommandHandler(UserDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        user.IsActive = true;
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
EOF

# GetUserByFirebaseUserIdQueryHandler.cs
cat > "$BASE/GetUserByFirebaseUserIdQueryHandler.cs" <<EOF
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Queries;
using UserService.Infrastructure.Persistence;
using Shared.Contracts.Users;

namespace UserService.Application.Handlers;

/// <summary>
/// Handles retrieval of user by FirebaseUserId.
/// </summary>
public class GetUserByFirebaseUserIdQueryHandler : IRequestHandler<GetUserByFirebaseUserIdQuery, UserDto>
{
    private readonly UserDbContext _context;

    public GetUserByFirebaseUserIdQueryHandler(UserDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(GetUserByFirebaseUserIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Where(x => x.FirebaseUserId == request.FirebaseUserId)
            .Select(x => new UserDto
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Phone = x.Phone
            }).FirstOrDefaultAsync(cancellationToken);

        return user ?? throw new KeyNotFoundException("User not found");
    }
}
EOF

# GetUserByUserIdQueryHandler.cs
cat > "$BASE/GetUserByUserIdQueryHandler.cs" <<EOF
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

    public GetUserByUserIdQueryHandler(UserDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(GetUserByUserIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Where(x => x.Id == request.UserId)
            .Select(x => new UserDto
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Phone = x.Phone
            }).FirstOrDefaultAsync(cancellationToken);

        return user ?? throw new KeyNotFoundException("User not found");
    }
}
EOF

# SearchUsersQueryHandler.cs
cat > "$BASE/SearchUsersQueryHandler.cs" <<EOF
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Queries;
using UserService.Infrastructure.Persistence;
using Shared.Contracts.Users;

namespace UserService.Application.Handlers;

/// <summary>
/// Handles user search with pagination and filters.
/// </summary>
public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, PaginatedResult<UserDto>>
{
    private readonly UserDbContext _context;

    public SearchUsersQueryHandler(UserDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<UserDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Name))
            query = query.Where(x => x.Name.Contains(request.Name));

        if (!string.IsNullOrWhiteSpace(request.Email))
            query = query.Where(x => x.Email.Contains(request.Email));

        if (!string.IsNullOrWhiteSpace(request.Phone))
            query = query.Where(x => x.Phone!.Contains(request.Phone));

        var total = await query.CountAsync(cancellationToken);

        var users = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new UserDto
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Phone = x.Phone
            }).ToListAsync(cancellationToken);

        return new PaginatedResult<UserDto>
        {
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize,
            Items = users
        };
    }
}
EOF

echo "✅ All UserService Handlers generated successfully!"
