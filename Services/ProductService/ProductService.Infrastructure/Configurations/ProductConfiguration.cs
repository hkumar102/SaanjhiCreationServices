using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        // Basic Properties
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.Brand)
            .HasMaxLength(100);

        builder.Property(p => p.Designer)
            .HasMaxLength(100);

        builder.Property(p => p.SKU)
            .HasMaxLength(50);

        // Pricing Properties
        builder.Property(p => p.PurchasePrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.RentalPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.SecurityDeposit)
            .HasColumnType("decimal(18,2)");

        // Specification Properties
        builder.Property(p => p.AvailableSizes)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

        builder.Property(p => p.AvailableColors)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

        builder.Property(p => p.Material)
            .HasMaxLength(100);

        builder.Property(p => p.CareInstructions)
            .HasMaxLength(500);

        builder.Property(p => p.Occasion)
            .HasMaxLength(100);

        builder.Property(p => p.Season)
            .HasMaxLength(50);

        // Category relationship
        builder.HasIndex(p => p.CategoryId);
        builder.Property(p => p.CategoryId).IsRequired();

        // Navigation Properties
        builder.HasMany(p => p.Media)
            .WithOne(m => m.Product)
            .HasForeignKey(m => m.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.InventoryItems)
            .WithOne(i => i.Product)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}