#!/bin/bash

echo "🚀 Generating full MediatR structure for CategoryService (with centralized Handlers)..."

BASE="./services/CategoryService/CategoryService.Application"

mkdir -p \
  "$BASE/Commands" \
  "$BASE/Queries" \
  "$BASE/Handlers" \
  "$BASE/Validators"

### ---- COMMANDS ----

cat > "$BASE/Commands/CreateCategoryCommand.cs" <<EOF
using MediatR;
using Shared.Contracts.Categories;

namespace CategoryService.Application.Commands;

/// <summary>
/// Command to create a new category.
/// </summary>
public class CreateCategoryCommand : IRequest<CategoryDto>
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
EOF

cat > "$BASE/Commands/UpdateCategoryCommand.cs" <<EOF
using MediatR;
using Shared.Contracts.Categories;

namespace CategoryService.Application.Commands;

/// <summary>
/// Command to update a category.
/// </summary>
public class UpdateCategoryCommand : IRequest<CategoryDto>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
EOF

cat > "$BASE/Commands/DeleteCategoryCommand.cs" <<EOF
using MediatR;

namespace CategoryService.Application.Commands;

/// <summary>
/// Command to delete a category.
/// </summary>
public class DeleteCategoryCommand : IRequest
{
    public Guid Id { get; set; }
}
EOF

### ---- QUERIES ----

cat > "$BASE/Queries/GetAllCategoriesQuery.cs" <<EOF
using MediatR;
using Shared.Contracts.Categories;

namespace CategoryService.Application.Queries;

/// <summary>
/// Query to get all categories.
/// </summary>
public class GetAllCategoriesQuery : IRequest<List<CategoryDto>> { }
EOF

cat > "$BASE/Queries/GetCategoryByIdQuery.cs" <<EOF
using MediatR;
using Shared.Contracts.Categories;

namespace CategoryService.Application.Queries;

/// <summary>
/// Query to get a category by Id.
/// </summary>
public class GetCategoryByIdQuery : IRequest<CategoryDto>
{
    public Guid Id { get; set; }
}
EOF

cat > "$BASE/Queries/GetCategoryByNameQuery.cs" <<EOF
using MediatR;
using Shared.Contracts.Categories;

namespace CategoryService.Application.Queries;

/// <summary>
/// Query to get a category by Name.
/// </summary>
public class GetCategoryByNameQuery : IRequest<CategoryDto>
{
    public string Name { get; set; } = null!;
}
EOF

### ---- HANDLERS ----

cat > "$BASE/Handlers/CreateCategoryCommandHandler.cs" <<EOF
using MediatR;
using CategoryService.Domain.Entities;
using CategoryService.Infrastructure.Persistence;
using Shared.Contracts.Categories;
using CategoryService.Application.Commands;

namespace CategoryService.Application.Handlers;

/// <summary>
/// Handler to process CreateCategoryCommand.
/// </summary>
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly CategoryDbContext _dbContext;

    public CreateCategoryCommandHandler(CategoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description
        };

        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }
}
EOF

cat > "$BASE/Handlers/UpdateCategoryCommandHandler.cs" <<EOF
using MediatR;
using Microsoft.EntityFrameworkCore;
using CategoryService.Infrastructure.Persistence;
using Shared.Contracts.Categories;
using CategoryService.Application.Commands;

namespace CategoryService.Application.Handlers;

/// <summary>
/// Handler to process UpdateCategoryCommand.
/// </summary>
public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly CategoryDbContext _dbContext;

    public UpdateCategoryCommandHandler(CategoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FindAsync(new object[] { request.Id }, cancellationToken);
        if (category == null)
            throw new KeyNotFoundException("Category not found");

        category.Name = request.Name;
        category.Description = request.Description;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }
}
EOF

cat > "$BASE/Handlers/DeleteCategoryCommandHandler.cs" <<EOF
using MediatR;
using CategoryService.Infrastructure.Persistence;
using CategoryService.Application.Commands;

namespace CategoryService.Application.Handlers;

/// <summary>
/// Handler to process DeleteCategoryCommand.
/// </summary>
public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly CategoryDbContext _dbContext;

    public DeleteCategoryCommandHandler(CategoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FindAsync(new object[] { request.Id }, cancellationToken);
        if (category == null)
            throw new KeyNotFoundException("Category not found");

        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
EOF

cat > "$BASE/Handlers/GetAllCategoriesQueryHandler.cs" <<EOF
using MediatR;
using Microsoft.EntityFrameworkCore;
using CategoryService.Infrastructure.Persistence;
using Shared.Contracts.Categories;
using CategoryService.Application.Queries;

namespace CategoryService.Application.Handlers;

/// <summary>
/// Handler to process GetAllCategoriesQuery.
/// </summary>
public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
{
    private readonly CategoryDbContext _dbContext;

    public GetAllCategoriesQueryHandler(CategoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Categories
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToListAsync(cancellationToken);
    }
}
EOF

cat > "$BASE/Handlers/GetCategoryByIdQueryHandler.cs" <<EOF
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Categories;
using CategoryService.Infrastructure.Persistence;
using CategoryService.Application.Queries;

namespace CategoryService.Application.Handlers;

/// <summary>
/// Handler to process GetCategoryByIdQuery.
/// </summary>
public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly CategoryDbContext _dbContext;

    public GetCategoryByIdQueryHandler(CategoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories
            .Where(c => c.Id == request.Id)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            })
            .FirstOrDefaultAsync(cancellationToken);

        return category ?? throw new KeyNotFoundException("Category not found");
    }
}
EOF

cat > "$BASE/Handlers/GetCategoryByNameQueryHandler.cs" <<EOF
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Categories;
using CategoryService.Infrastructure.Persistence;
using CategoryService.Application.Queries;

namespace CategoryService.Application.Handlers;

/// <summary>
/// Handler to process GetCategoryByNameQuery.
/// </summary>
public class GetCategoryByNameQueryHandler : IRequestHandler<GetCategoryByNameQuery, CategoryDto>
{
    private readonly CategoryDbContext _dbContext;

    public GetCategoryByNameQueryHandler(CategoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CategoryDto> Handle(GetCategoryByNameQuery request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories
            .Where(c => c.Name == request.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            })
            .FirstOrDefaultAsync(cancellationToken);

        return category ?? throw new KeyNotFoundException("Category not found");
    }
}
EOF

echo "✅ All CQRS files for CategoryService created successfully with centralized Handlers folder."
