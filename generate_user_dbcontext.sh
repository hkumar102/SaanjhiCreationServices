#!/bin/bash

echo "🛠 Creating UserDbContext..."

persistence_dir="./services/UserService/UserService.Infrastructure/Persistence"
mkdir -p "$persistence_dir"

cat > "$persistence_dir/UserDbContext.cs" <<EOF
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Configurations;

namespace Services.UserService.Infrastructure.Persistence;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<ShippingAddress> ShippingAddresses => Set<ShippingAddress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new ShippingAddressConfiguration());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            var now = DateTime.UtcNow;

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.ModifiedAt = now;
                    // Optionally set CreatedBy and ModifiedBy from current user
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedAt = now;
                    // Optionally set ModifiedBy
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
EOF

echo "✅ Created: $persistence_dir/UserDbContext.cs"
