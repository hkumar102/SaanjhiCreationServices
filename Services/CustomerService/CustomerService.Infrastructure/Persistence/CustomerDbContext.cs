using Microsoft.EntityFrameworkCore;
using CustomerService.Domain.Entities;
using CustomerService.Infrastructure.Persistence.Configurations;
using Shared.Domain.Entities;
using Shared.Application.Interfaces;

namespace CustomerService.Infrastructure.Persistence
{
    public class CustomerDbContext(DbContextOptions<CustomerDbContext> options, ICurrentUserService currentUserService) : DbContext(options)
    {
        public DbSet<Customer> Customers { get; set; } = default!;
        public DbSet<Address> Addresses { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new AddressConfiguration());
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
                        entry.Entity.CreatedBy = currentUserService.UserId; // Set to null or current user ID if available
                        // Optionally set CreatedBy and ModifiedBy from current user
                        break;

                    case EntityState.Modified:
                        entry.Entity.ModifiedAt = now;
                        entry.Entity.ModifiedBy = currentUserService.UserId;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}