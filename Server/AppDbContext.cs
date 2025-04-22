using Microsoft.EntityFrameworkCore;

namespace Server
{
    internal class AppDbContext(IServerConfigService configService) : DbContext(configService.DbConfig.DbOptions)
    {
        public DbSet<User> Users { get; set; }
    }
}
