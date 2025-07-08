using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Configurations;

public class ProductMediaConfiguration : IEntityTypeConfiguration<ProductMedia>
{
    public void Configure(EntityTypeBuilder<ProductMedia> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Url)
            .IsRequired();

        builder.Property(m => m.MediaType)
            .HasConversion<int>();

        builder.HasOne(m => m.Product)
            .WithMany(p => p.Media)
            .HasForeignKey(m => m.ProductId);
    }
}