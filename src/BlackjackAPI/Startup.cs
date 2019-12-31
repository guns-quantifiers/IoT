using BlackjackAPI.Middleware;
using Core.Components;
using Core.Exceptions;
using Core.Settings;
using DbDataSource;
using FileSave;
using Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog.Web;
using Strategies;
using Strategies.BetStrategy;
using Strategies.StrategyContexts;
using System;
using System.Threading.Tasks;

namespace BlackjackAPI
{
    public class Startup
    {
        internal const string LoggerName = "BerryjackLogger";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddRouteAnalyzer();
            NLog.ILogger logger =
                NLogBuilder.
                    ConfigureNLog(Configuration.GetValue<string>("Logging:NlogConfiguration"))
                    .GetLogger(LoggerName);

            services.AddSingleton<NLog.ILogger>(logger);
            services.AddHealthChecks();
            services.AddControllers();
            services.AddSingleton<IGamesRepository, GamesRepository>();
            services.AddSingleton<IStrategyContext, UstonSSStrategyContext>();
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<IGameStorage, FileGameStorage>();
            services.AddSingleton<IStrategyProvider, ChartedBasicStrategy>();
            services.AddSingleton<IBetMultiplierCalculator, BetMultiplierCalculator>();
            RegisterPersistenceSettings(services);
            ConventionRegistry.Register("EnumStringConvention", new ConventionPack { new EnumRepresentationConvention(BsonType.String) }, t => true);

            void RegisterPersistenceSettings(IServiceCollection services)
            {
                if (Configuration.GetSection("PersistenceSettings:Database").Exists())
                {
                    logger.Debug("Configuring connection to database.");
                    services.Configure<DbPersistenceSettings>(Configuration.GetSection("PersistenceSettings:Database"));
                    services.PostConfigure<DbPersistenceSettings>(dbPersistenceSettings =>
                    {
                        dbPersistenceSettings.ConnectionString = Configuration.GetSection("MONGO_CONNECTION_STRING").Value;
                    });
                }
                else if (Configuration.GetSection("PersistenceSettings:FilePath").Exists())
                {
                    logger.Debug("Configuring local storage usage.");
                    services.Configure<LocalPersistenceSettings>(Configuration.GetSection("PersistenceSettings"));
                }
                else
                {
                    throw new ConfigurationException("Could not find any settings for saving games.");
                }
            }
        }

        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            NLog.ILogger logger,
            IHostApplicationLifetime applicationLifetime)
        {
            if (env.EnvironmentName.Equals("Development", StringComparison.OrdinalIgnoreCase))
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                ResponseWriter = WriteResponse
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            applicationLifetime.ApplicationStarted.Register(() =>
            {
                if (logger.IsInfoEnabled)
                    logger.Info($"{Environment.NewLine}Application started successfully!{Environment.NewLine}");
            });
        }


        private static Task WriteResponse(HttpContext httpContext,
            HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";

            var json = new JObject(
                    new JProperty("status", result.Status.ToString())
                );
            return httpContext.Response.WriteAsync(
                json.ToString(Formatting.Indented));
        }
    }
}
