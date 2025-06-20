#!/bin/bash

echo "🧱 Generating Category entity, configuration, and DTO..."

# 1. Category entity
entity_dir="./services/CategoryService/CategoryService.Domain/Entities"
mkdir -p "$entity_dir"

cat > "$entity_dir/Category.cs" <<EOF
using Shared.Domain.Entities;

namespace CategoryService.Domain.Entities;

public class Category : AuditableEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}
EOF

# 2. Category configuration
config_dir="./services/CategoryService/CategoryService.Domain/Configurations"
mkdir -p "$config_dir"

cat > "$config_dir/CategoryConfiguration.cs" <<EOF
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CategoryService.Domain.Entities;

namespace CategoryService.Domain.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(255);
    }
}
EOF

# 3. Category DTO
dto_dir="./Shared/Contracts/Categories"
mkdir -p "$dto_dir"

cat > "$dto_dir/CategoryDto.cs" <<EOF
namespace Shared.Contracts.Categories;

public class CategoryDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}
EOF

echo "✅ Created: $entity_dir/Category.cs"
echo "✅ Created: $config_dir/CategoryConfiguration.cs"
echo "✅ Created: $dto_dir/CategoryDto.cs"
