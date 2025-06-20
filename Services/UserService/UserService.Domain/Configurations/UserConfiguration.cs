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
