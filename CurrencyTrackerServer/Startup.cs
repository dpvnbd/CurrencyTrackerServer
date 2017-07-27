using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyTrackerServer.BittrexService.Concrete;
using CurrencyTrackerServer.BittrexService.Entities;
using CurrencyTrackerServer.Infrastructure;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Concrete;
using CurrencyTrackerServer.Infrastructure.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CurrencyTrackerServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.

            services.AddWebSocketManager();
            services.AddMvc();

            


            services.AddTransient<DbContext, BittrexContext>();
            services.AddTransient<IDataSource<List<BittrexApiData>>, BittrexApiDataSource>();

            var provider = services.BuildServiceProvider();
            var connectionManager = provider.GetRequiredService<WebSocketConnectionManager>();
            var handler = new NotificationsMessageHandler(connectionManager);

            services.AddSingleton<INotifier<Change>>(handler);
            services.AddSingleton<WebSocketHandler>(handler);
            
            services
                .AddTransient<IChangeMonitor<List<Change>>, BittrexChangeMonitor<Repository<CurrencyStateEntity>,
                    Repository<ChangeHistoryEntryEntity>>>();
            services.AddSingleton<BittrexTimerWorker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider, IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseWebSockets();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            
            
            app.MapWebSocketManager("/notifications", serviceProvider.GetRequiredService<WebSocketHandler>());

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<DbContext>();
                context.Database.Migrate();
            }
        }
    }
}