using AspNetCore.RouteAnalyzer;
using BlackjackAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace BlackjackAPI
{
    public class Startup
    {
        private const string DbName = "Blackjack";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddRouteAnalyzer();

            services.AddSingleton<GameContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            var context = app.ApplicationServices.GetService<GameContext>();
            AddTestData(context);

            app.UseHttpsRedirection();
            app.UseMvc(routes =>
            {
                routes.MapRouteAnalyzer("/routes"); // Add
            });
        }

        private void AddTestData(GameContext context)
        {
            var startDeals = new List<Deal>()
            {
                new Deal()
                {
                    Id = new Guid(),
                    CroupierHand = new Hand(){CardType.Queen},
                    PlayerHand = new Hand(){CardType.Five, CardType.Jack}
                },
                new Deal()
                {
                    Id = new Guid(),
                    CroupierHand = new Hand(){CardType.Ace},
                    PlayerHand = new Hand(){CardType.Two, CardType.Six}
                }
            };
            context.Games.Add(
                new Game()
                {
                    Id = new Guid(),
                    History = startDeals
                });
        }
    }
}
