using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace BlackjackAPI.DesignMigrations
{
    public class ConfigurationService : IConfigurationService
    {
        public string CurrentDirectory { get; private set; }

        public ConfigurationService(string currentDirectory) => CurrentDirectory = currentDirectory;

        public IConfiguration GetConfiguration()
        {
            CurrentDirectory ??= Directory.GetCurrentDirectory();
            return new ConfigurationBuilder()
                .SetBasePath(CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
