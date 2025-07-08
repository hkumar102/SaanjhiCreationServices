using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MediaService.Domain.Entities;

namespace MediaService.Domain.Configurations;

public class MediaConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.FileName).IsRequired().HasMaxLength(255);
        builder.Property(m => m.Url).IsRequired();
        builder.Property(m => m.ContentType).IsRequired().HasMaxLength(100);
        builder.Property(m => m.Size).IsRequired();
    }
}
