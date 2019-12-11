using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NLog.Web;
using Core.Components;
using BlackjackAPI.Services;
using Strategies;
using Strategies.GameContexts;
using BlackjackAPI.Middleware;
using Core.Exceptions;
using Logging;
using Core.Settings;
using DbDataSource;
using Microsoft.EntityFrameworkCore;
using Strategies.BetStrategy;

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
            services.AddSingleton<IGameContext, UstonSSGameContext>();
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<IGameSaver, GameSaver>();
            services.AddSingleton<IStrategyProvider, ChartedBasicStrategy>();
            services.AddSingleton<IBetMultiplierCalculator, BetMultiplierCalculator>();
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

            var gameContext = app.ApplicationServices.GetService<IGameContext>();
            gameContext.Initialize();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            applicationLifetime.ApplicationStarted.Register(() =>
            {
                if(logger.IsInfoEnabled)
                    logger.Info($"{Environment.NewLine}Application started successfully!{Environment.NewLine}");
            });
        }

        private void RegisterPersistenceSettings(IServiceCollection services)
        {
            if (Configuration.GetSection("PersistenceSettings:ConnectionString").Exists())
            {
                services.AddDbContext<GameContext>(opts => opts.UseSqlServer(Configuration["PersistenceSettings:ConnectionString:GameDb"]));
            }
            else if (Configuration.GetSection("PersistenceSettings:FilePath").Exists())
            {
                services.Configure<PersistenceSettings>(Configuration.GetSection("PersistenceSettings"));
            }
            throw new ConfigurationException("Could not find any settings for saving games.");
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
