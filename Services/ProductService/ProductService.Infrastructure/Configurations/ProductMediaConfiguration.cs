using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Configurations;

public class ProductMediaConfiguration : IEntityTypeConfiguration<ProductMedia>
{
    public void Configure(EntityTypeBuilder<ProductMedia> builder)
    {
        builder.HasKey(m => m.Id);

        // Required Properties
        builder.Property(m => m.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(m => m.MediaType)
            .HasConversion<int>()
            .IsRequired();

        // Optional Properties
        builder.Property(m => m.PublicId)
            .HasMaxLength(200);

        builder.Property(m => m.Color)
            .HasMaxLength(50);

        builder.Property(m => m.Size)
            .HasMaxLength(20);

        builder.Property(m => m.MediaPurpose)
            .HasMaxLength(50);

        builder.Property(m => m.AltText)
            .HasMaxLength(500);

        // Indexes for performance
        builder.HasIndex(m => m.ProductId);
        builder.HasIndex(m => new { m.ProductId, m.Color });
        builder.HasIndex(m => new { m.ProductId, m.MediaPurpose });
        builder.HasIndex(m => new { m.ProductId, m.IsGeneric });
        builder.HasIndex(m => new { m.ProductId, m.Color, m.MediaPurpose });

        // Relationships
        builder.HasOne(m => m.Product)
            .WithMany(p => p.Media)
            .HasForeignKey(m => m.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}