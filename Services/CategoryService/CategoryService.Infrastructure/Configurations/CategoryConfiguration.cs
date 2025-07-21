using CategoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Infrastructure.Configurations;

namespace CategoryService.Infrastructure.Configurations;

public class CategoryConfiguration : BaseEntityConfiguration<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        base.Configure(builder);
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Description).HasMaxLength(500);
    }
}
