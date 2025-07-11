using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ProductService.Infrastructure.Persistence;

namespace ProductService.Infrastructure.Data;

public class ProductDbContextFactory : IDesignTimeDbContextFactory<ProductDbContext>
{
    public ProductDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ProductDbContext>();
        
        // Use the same connection string as configured in appsettings.json
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=saanjhicreation;Username=saanjhiuser;Password=saanjhiuser");
        
        return new ProductDbContext(optionsBuilder.Options);
    }
}
