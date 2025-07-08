namespace UserService.Domain.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entities;

public class AuthProviderConfiguration : IEntityTypeConfiguration<AuthProvider>
{
    public void Configure(EntityTypeBuilder<AuthProvider> builder)
    {

        builder.HasKey(p => p.Id);

        builder.Property(p => p.ProviderId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Uid)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.DisplayName).HasMaxLength(100);
        builder.Property(p => p.Email).HasMaxLength(150);
        builder.Property(p => p.PhotoUrl).HasMaxLength(300);

        builder
            .HasOne(p => p.User)
            .WithMany(u => u.Providers)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}