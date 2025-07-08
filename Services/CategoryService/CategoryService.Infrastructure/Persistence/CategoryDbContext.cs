using CategoryService.Domain.Entities;
using CategoryService.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CategoryService.Infrastructure.Persistence;

public class CategoryDbContext(DbContextOptions<CategoryDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
