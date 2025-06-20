using Microsoft.EntityFrameworkCore;
using CategoryService.Domain.Entities;
using CategoryService.Domain.Configurations;
using Shared.Domain.Entities;
using Shared.Authentication;

namespace CategoryService.Infrastructure.Persistence;

public class CategoryDbContext : DbContext
{
    private readonly ICurrentUserService _currentUser;

    public CategoryDbContext(DbContextOptions<CategoryDbContext> options, ICurrentUserService currentUser)
        : base(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            var now = DateTime.UtcNow;
            var userId = _currentUser.FirebaseUserId;

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.ModifiedAt = now;
                    entry.Entity.ModifiedBy = userId;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedAt = now;
                    entry.Entity.ModifiedBy = userId;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
