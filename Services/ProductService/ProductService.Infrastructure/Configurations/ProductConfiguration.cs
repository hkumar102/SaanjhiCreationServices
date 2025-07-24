using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Entities;
using Shared.Infrastructure.Configurations;

namespace ProductService.Infrastructure.Configurations;

public class ProductConfiguration : BaseEntityConfiguration<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);

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

        //We need make sure SKU is unique across products
        builder.HasIndex(p => p.SKU)
            .IsUnique();
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
            .HasColumnType("text[]");

        builder.Property(p => p.AvailableColors)
            .HasColumnType("text[]");

        builder.Property(p => p.Material)
            .HasMaxLength(100);

        builder.Property(p => p.CareInstructions)
            .HasMaxLength(500);

        builder.Property(p => p.Occasion)
            .HasColumnType("text[]");

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