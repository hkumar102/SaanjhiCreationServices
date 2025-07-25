using Microsoft.EntityFrameworkCore;
using RentalService.Domain.Entities;
using Shared.Application.Interfaces;
using Shared.Domain.Entities;

namespace RentalService.Infrastructure.Persistence;

public class RentalDbContext(DbContextOptions<RentalDbContext> options, ICurrentUserService currentUser) : DbContext(options)
{
    public DbSet<Rental> Rentals => Set<Rental>();
    public DbSet<RentalTimeline> RentalTimelines => Set<RentalTimeline>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RentalDbContext).Assembly);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            var now = DateTime.UtcNow;

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.ModifiedAt = now;
                    entry.Entity.CreatedBy = currentUser?.UserId; // Set to null or current user ID if available
                    // Optionally set CreatedBy and ModifiedBy from current user
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedAt = now;
                    entry.Entity.ModifiedBy = currentUser?.UserId;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}