using CategoryService.Domain.Entities;
using CategoryService.Domain.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CategoryService.Infrastructure.Persistence;

public class CategoryDbContext : DbContext
{
    public CategoryDbContext(DbContextOptions<CategoryDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
