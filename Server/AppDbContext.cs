using Microsoft.EntityFrameworkCore;

namespace Server;

internal class AppDbContext(IServerConfigService configService) : DbContext((configService.DbConfig ?? throw new ArgumentNullException(nameof(configService))).DbOptions)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Media>()
            .HasGeneratedTsVectorColumn(
                p => p.SearchVector,
                "english",
                p => new { p.Name })
            .HasIndex(p => p.SearchVector)
            .HasMethod("GIN");
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Media> Medias { get; set; }
}