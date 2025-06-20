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
