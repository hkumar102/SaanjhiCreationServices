using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.Entities;

namespace Shared.Infrastructure.Configurations;

/// <summary>
/// Base configuration for all entities that inherit from BaseEntity
/// </summary>
public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        // Primary Key
        builder.HasKey(e => e.Id);
        
        // Id Generation
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // Audit Fields
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(256);

        builder.Property(e => e.ModifiedAt);

        builder.Property(e => e.ModifiedBy)
            .HasMaxLength(256);

        // Soft Delete
        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.DeletedAt);

        builder.Property(e => e.DeletedBy)
            .HasMaxLength(256);

        // Concurrency Control
        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(e => !e.IsDeleted);

        // Indexes
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.IsDeleted);
    }
}
