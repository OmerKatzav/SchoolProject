using Microsoft.EntityFrameworkCore;

namespace Server
{
    internal class DbConfig(DbContextOptions dbOptions)
    {
        public DbContextOptions DbOptions { get; } = dbOptions;
    }
}
