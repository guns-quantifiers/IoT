using System;
using DbDataSource;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlackjackAPI.DesignMigrations
{
    public class DesignServicesProvider
    {
        public IServiceProvider ServiceProvider { get; }
        public string CurrentDirectory { get; set; }

        public DesignServicesProvider()
        {
            IServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IConfigurationService, ConfigurationService>
            (provider => new ConfigurationService(CurrentDirectory));

            // Register DbContext class
            services.AddTransient(provider =>
            {
                var connectionString = provider.GetService<IConfigurationService>()
                    .GetConfiguration()
                    [$"PersistenceSettings:ConnectionString:{nameof(GameContext)}"];
                var optionsBuilder = new DbContextOptionsBuilder<GameContext>();
                optionsBuilder.UseSqlServer(connectionString, builder => builder.MigrationsAssembly("DbDataSource"));
                return new GameContext(optionsBuilder.Options);
            });
        }
    }
}
