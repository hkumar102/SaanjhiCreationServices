using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalService.Domain.Entities;

namespace RentalService.Infrastructure.Persistence.Configurations
{
    public class RentalTimelineConfiguration : IEntityTypeConfiguration<RentalTimeline>
    {
        public void Configure(EntityTypeBuilder<RentalTimeline> builder)
        {
            builder.Property(rt => rt.Status).IsRequired();
            builder.Property(rt => rt.Notes).HasMaxLength(500);
            builder.HasIndex(rt => rt.RentalId);
            builder.HasOne(rt => rt.Rental)
                .WithMany(r => r.Timelines)
                .HasForeignKey(rt => rt.RentalId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.ToTable("RentalTimeline");
        }
    }
}
