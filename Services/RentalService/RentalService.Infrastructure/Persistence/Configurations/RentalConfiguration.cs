using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalService.Domain.Entities;
using Shared.Infrastructure.Configurations;

namespace RentalService.Infrastructure.Persistence.Configurations;

public class RentalConfiguration : BaseEntityConfiguration<Rental>
{
    public override void Configure(EntityTypeBuilder<Rental> builder)
    {
        base.Configure(builder);
        builder.HasKey(r => r.Id);

        builder.Property(r => r.RentalPrice).HasColumnType("decimal(18,2)");
        builder.Property(r => r.SecurityDeposit).HasColumnType("decimal(18,2)");

        builder.Property(r => r.Status)
            .HasConversion<int>()
            .IsRequired();

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

        builder.HasMany(r => r.Timelines)
            .WithOne()
            .HasForeignKey(rt => rt.RentalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Rentals");
    }
}