using Microsoft.EntityFrameworkCore;
using MediaService.Domain.Entities;

namespace MediaService.Infrastructure.Persistence;

public class MediaDbContext(DbContextOptions<MediaDbContext> options) : DbContext(options)
{
    public DbSet<Media> Medias { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MediaDbContext).Assembly);
    }
}