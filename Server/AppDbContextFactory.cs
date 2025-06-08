using dotenv.net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Server;

internal class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var dotenvVars = DotEnv.Read();
        return new AppDbContext(new ServerConfigService{ DbConfig = new DbConfig(new DbContextOptionsBuilder().UseNpgsql(dotenvVars["NPGSQL_CONNECTION_STRING"]).Options) });
    }
}