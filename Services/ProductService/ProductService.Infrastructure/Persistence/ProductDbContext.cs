// ProductService.Infrastructure/Persistence/ProductDbContext.cs
using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Configurations;
using Shared.Domain.Entities;

namespace ProductService.Infrastructure.Persistence;

public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductMedia> ProductMedia => Set<ProductMedia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
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
                    entry.Entity.CreatedBy = "system"; // Set to null or current user ID if available
                    // Optionally set CreatedBy and ModifiedBy from current user
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedAt = now;
                    entry.Entity.ModifiedBy = "system";
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}