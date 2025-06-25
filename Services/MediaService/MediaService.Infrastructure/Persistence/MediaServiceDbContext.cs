using Microsoft.EntityFrameworkCore;
using MediaService.Domain.Entities;

namespace MediaService.Infrastructure.Persistence;

public class MediaServiceDbContext : DbContext
{
    public MediaServiceDbContext(DbContextOptions<MediaServiceDbContext> options) : base(options) { }

    public DbSet<Media> Medias { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MediaServiceDbContext).Assembly);
    }
}