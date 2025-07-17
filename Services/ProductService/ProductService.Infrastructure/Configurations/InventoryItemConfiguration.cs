using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Entities;
using Shared.Infrastructure.Configurations;

namespace ProductService.Infrastructure.Configurations;

public class InventoryItemConfiguration : BaseEntityConfiguration<InventoryItem>
{
    public override void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        base.Configure(builder);
        builder.HasKey(i => i.Id);

        // Physical Properties
        builder.Property(i => i.Size)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(i => i.Color)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.SerialNumber)
            .HasMaxLength(100);

        builder.Property(i => i.BarcodeImageBase64)
            .HasColumnType("text");

        // Status and Condition as integers
        builder.Property(i => i.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(i => i.Condition)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(i => i.ConditionNotes)
            .HasMaxLength(500);

        // Financial Properties
        builder.Property(i => i.AcquisitionCost)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        // Location Properties
        builder.Property(i => i.WarehouseLocation)
            .HasMaxLength(100);

        builder.Property(i => i.StorageNotes)
            .HasMaxLength(500);

        // Retirement Properties
        builder.Property(i => i.RetirementReason)
            .HasMaxLength(500);

        // Indexes for performance
        builder.HasIndex(i => i.ProductId);
        builder.HasIndex(i => i.SerialNumber);
        builder.HasIndex(i => new { i.ProductId, i.Size, i.Color });
        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.Condition);
        builder.HasIndex(i => i.IsRetired);

        // Product relationship
        builder.HasOne(i => i.Product)
            .WithMany(p => p.InventoryItems)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore calculated properties
        builder.Ignore(i => i.IsAvailable);
        builder.Ignore(i => i.DaysSinceLastRented);
    }
}
