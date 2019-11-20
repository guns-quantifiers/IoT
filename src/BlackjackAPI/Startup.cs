using AspNetCore.RouteAnalyzer;
using BlackjackAPI.Models;
using BlackjackAPI.Services;
using BlackjackAPI.Strategies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace BlackjackAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouteAnalyzer();
            services.AddHealthChecks();
            services.AddSingleton<IGameContext, UstonSSGameContext>();
            services.AddSingleton<IGameSaver, GameSaver>();
            services.AddSingleton<IStrategyProvider, ChartedBasicStrategy>();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName.Equals("Development", System.StringComparison.OrdinalIgnoreCase))
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
        }

        private static Task WriteResponse(HttpContext httpContext,
            HealthReport result)
        {
            httpContext.Response.ContentType = "application/json";

            var json = new JObject(
                new JProperty("status", result.Status.ToString())
                //new JProperty("results", new JObject(result.Entries.Select(pair =>
                //    new JProperty(pair.Key, new JObject(
                //        new JProperty("status", pair.Value.Status.ToString()),
                //        new JProperty("description", pair.Value.Description),
                //        new JProperty("data", new JObject(pair.Value.Data.Select(
                //            p => new JProperty(p.Key, p.Value)))))))))
                );
            return httpContext.Response.WriteAsync(
                json.ToString(Formatting.Indented));
        }
    }
}
