using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace BlackjackAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            NLog.ILogger logger = null;
            try
            {
                WebHost.CreateDefaultBuilder(args)
                    .ConfigureLogging((context, logging) =>
                    {
                        logging.ClearProviders();
                        logger = NLogBuilder.ConfigureNLog(context.Configuration.GetValue<string>("Logging:NlogConfiguration")).GetLogger(Startup.LoggerName);
                    })
                    .UseStartup<Startup>()
                    .UseNLog()
                    .Build()
                    .Run();
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Program start error");
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }

        }
    }
}
