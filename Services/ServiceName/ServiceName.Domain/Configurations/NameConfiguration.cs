using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using services.ServiceName.ServiceName.Domain.Entities;

namespace services.ServiceName.ServiceName.Domain.Configurations;

public class NameConfiguration : IEntityTypeConfiguration<Name>
{
    public void Configure(EntityTypeBuilder<Name> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
    }
}
