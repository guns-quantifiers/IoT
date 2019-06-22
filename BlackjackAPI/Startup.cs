using AspNetCore.RouteAnalyzer;
using BlackjackAPI.Models;
using BlackjackAPI.Services;
using BlackjackAPI.Strategies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddRouteAnalyzer();

            services.AddSingleton<IGameContext, UstonSSGameContext>();
            services.AddSingleton<IGameSaver, GameSaver>();
            services.AddSingleton<IStrategyProvider, ChartedBasicStrategy>();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseMiddleware<ErrorHandlingMiddleware>();

            var gameContext = app.ApplicationServices.GetService<IGameContext>();
            gameContext.Initialize();

            app.UseMvc(routes =>
            {
                routes.MapRouteAnalyzer("/routes"); 
            });
        }
    }
}
