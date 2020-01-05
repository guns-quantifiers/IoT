using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.IO;

namespace BlackjackAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            NLog.ILogger logger = null;
            try
            {
                Host.CreateDefaultBuilder(args)
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                    .ConfigureLogging((context, logging) =>
                    {
                        logging.ClearProviders();
                        logger = NLogBuilder.ConfigureNLog(context.Configuration.GetValue<string>("Logging:NlogConfiguration")).GetLogger(Startup.LoggerName);
                    })
                    .ConfigureWebHostDefaults(webHostBuilder =>
                    {
                        webHostBuilder
                            .UseContentRoot(Directory.GetCurrentDirectory())
                            .UseIISIntegration()
                            .UseStartup<Startup>();
                    })
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
