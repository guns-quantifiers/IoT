using Microsoft.Extensions.Configuration;

namespace BlackjackAPI.DesignMigrations
{
    public interface IConfigurationService
    {
        IConfiguration GetConfiguration();
    }
}
