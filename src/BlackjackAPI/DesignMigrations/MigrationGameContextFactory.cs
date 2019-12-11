using System.IO;
using DbDataSource;
using Microsoft.EntityFrameworkCore.Design;

namespace BlackjackAPI.DesignMigrations
{
    public class MigrationGameContextFactory : IDesignTimeDbContextFactory<GameContext>
    {
        public GameContext CreateDbContext(string[] args)
        {
            var resolver = new DesignServicesProvider
            {
                CurrentDirectory = Directory.GetCurrentDirectory()
            };
            return resolver.ServiceProvider.GetService(typeof(GameContext)) as GameContext;

        }
    }
}
