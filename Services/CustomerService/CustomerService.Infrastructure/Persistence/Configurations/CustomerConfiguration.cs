using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CustomerService.Domain.Entities;

namespace CustomerService.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(c => c.UserId)
            .IsRequired(false);

        // Configure UserId as a foreign key to the Users table
        builder.HasIndex(c => c.UserId);
        builder.HasOne<User>() // No navigation property
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .HasPrincipalKey("Id") // User's PK
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false)
            .HasConstraintName("FK_Customer_User");

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.ModifiedAt)
            .IsRequired(false);

        builder.HasMany(c => c.Addresses)
            .WithOne(a => a.Customer)
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

// Dummy User class for configuration only (not referenced elsewhere)
public class User
{
    public Guid Id { get; set; }
}