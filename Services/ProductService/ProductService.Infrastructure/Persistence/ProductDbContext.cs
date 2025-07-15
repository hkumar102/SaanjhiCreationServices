// ProductService.Infrastructure/Persistence/ProductDbContext.cs
using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using Shared.Application.Interfaces;
using Shared.Infrastructure.Extensions;

namespace ProductService.Infrastructure.Persistence;

public class ProductDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;

    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    public ProductDbContext(DbContextOptions<ProductDbContext> options, ICurrentUserService currentUserService) 
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductMedia> ProductMedia => Set<ProductMedia>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Get current user from service, fallback to "system" for migrations/seeding
        var currentUser = _currentUserService?.UserId ?? "system";
        this.UpdateAuditFields(currentUser);
        
        return await base.SaveChangesAsync(cancellationToken);
    }
}