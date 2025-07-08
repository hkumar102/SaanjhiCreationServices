using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalService.Domain.Entities;

namespace RentalService.Infrastructure.Persistence.Configurations;

public class RentalConfiguration : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.RentalPrice).HasColumnType("decimal(18,2)");
        builder.Property(r => r.SecurityDeposit).HasColumnType("decimal(18,2)");

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Optional measurement fields
        builder.Property(r => r.Height).HasMaxLength(50);
        builder.Property(r => r.Chest).HasMaxLength(50);
        builder.Property(r => r.Waist).HasMaxLength(50);
        builder.Property(r => r.Hip).HasMaxLength(50);
        builder.Property(r => r.Shoulder).HasMaxLength(50);
        builder.Property(r => r.SleeveLength).HasMaxLength(50);
        builder.Property(r => r.Inseam).HasMaxLength(50);
        builder.Property(r => r.Notes).HasMaxLength(500);

        // Foreign key constraints — ProductId & CustomerId
        builder.HasIndex(r => r.ProductId);
        builder.HasIndex(r => r.CustomerId);

        // Optional: explicitly define FK constraints (without navigation)
        builder.Property(r => r.ProductId).IsRequired();
        builder.Property(r => r.CustomerId).IsRequired();

        builder.ToTable("Rentals");
    }
}