using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyTrackerServer.Areas.ChangeTracker.Infrastructure;
using CurrencyTrackerServer.ChangeTrackerService.Concrete;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Data;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Concrete;
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

            services.AddTransient<ChangeTrackerContext>();
            //services.AddTransient<IDataSource<IEnumerable<CurrencyChangeApiData>>, BittrexApiDataSource>();

            var provider = services.BuildServiceProvider();
            var connectionManager = provider.GetRequiredService<WebSocketConnectionManager>();
            var handler = new ChangeNotificationsMessageHandler(connectionManager);

            //services.AddSingleton<INotifier<Change>>(handler);
            services.AddSingleton<ChangeNotificationsMessageHandler>(handler);

            //services
            //    .AddTransient<IChangeMonitor<IEnumerable<Change>>, ChangeMonitor<Repository<CurrencyStateEntity>,
            //        Repository<ChangeHistoryEntryEntity>>>();

            services.AddSingleton(s => new BittrexTimerWorker(handler));
            services.AddSingleton(s => new PoloniexTimerWorker(handler));
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

            app.MapWebSocketManager("/changeNotifications",
                serviceProvider.GetRequiredService<ChangeNotificationsMessageHandler>());

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{area=ChangeTracker}/{controller=Home}/{action=Index}/{id?}");
            });


            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ChangeTrackerContext>();
                context.Database.Migrate();
            }
        }
    }
}