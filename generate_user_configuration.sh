#!/bin/bash

echo "🛠 Generating UserService entity configurations..."

config_dir="./services/UserService/UserService.Domain/Configurations"
mkdir -p "$config_dir"

# UserConfiguration.cs
cat > "$config_dir/UserConfiguration.cs" <<EOF
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.Domain.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FullName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(100);
        builder.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(20);
        builder.Property(x => x.PhotoUrl).HasMaxLength(255);
        builder.Property(x => x.Provider).IsRequired().HasMaxLength(50);

        builder.HasMany(x => x.Roles)
               .WithOne(x => x.User)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.ShippingAddresses)
               .WithOne(x => x.User)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
EOF

# RoleConfiguration.cs
cat > "$config_dir/RoleConfiguration.cs" <<EOF
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.Domain.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);

        builder.HasMany(x => x.UserRoles)
               .WithOne(x => x.Role)
               .HasForeignKey(x => x.RoleId);
    }
}
EOF

# UserRoleConfiguration.cs
cat > "$config_dir/UserRoleConfiguration.cs" <<EOF
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.Domain.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(x => new { x.UserId, x.RoleId });

        builder.HasOne(x => x.User)
               .WithMany(x => x.Roles)
               .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.Role)
               .WithMany(x => x.UserRoles)
               .HasForeignKey(x => x.RoleId);
    }
}
EOF

# ShippingAddressConfiguration.cs
cat > "$config_dir/ShippingAddressConfiguration.cs" <<EOF
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Entities;

namespace UserService.Domain.Configurations;

public class ShippingAddressConfiguration : IEntityTypeConfiguration<ShippingAddress>
{
    public void Configure(EntityTypeBuilder<ShippingAddress> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AddressLine1).IsRequired().HasMaxLength(150);
        builder.Property(x => x.AddressLine2).HasMaxLength(150);
        builder.Property(x => x.City).IsRequired().HasMaxLength(50);
        builder.Property(x => x.State).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ZipCode).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Country).IsRequired().HasMaxLength(50);

        builder.HasOne(x => x.User)
               .WithMany(x => x.ShippingAddresses)
               .HasForeignKey(x => x.UserId);
    }
}
EOF

echo "✅ All entity configurations created in: $config_dir"
