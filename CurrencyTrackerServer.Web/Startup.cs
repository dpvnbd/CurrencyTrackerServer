using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Data;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Bittrex;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.ProviderSpecific.Poloniex;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;
using CurrencyTrackerServer.Web.Infrastructure;
using CurrencyTrackerServer.Web.Infrastructure.Concrete;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CurrencyTrackerServer.Web
{
  public class Startup
  {
    private IHostingEnvironment _env;
    public IConfigurationRoot Configuration { get; }

    public Startup(IHostingEnvironment env)
    {
      _env = env;

      var builder = new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
        .AddEnvironmentVariables();
      Configuration = builder.Build();
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc();

      #region Persistence DI

      services.AddDbContext<ChangeTrackerContext>(options => options
        .UseSqlServer(Configuration.GetConnectionString("ChangesDb")), ServiceLifetime.Transient);

      services.AddSingleton<IDbContextFactory<DbContext>, DbFactory>();
      services.AddSingleton<DbContextFactoryOptions>(s =>
        new DbContextFactoryOptions() {ContentRootPath = _env.ContentRootPath, EnvironmentName = _env.EnvironmentName});
      services.AddSingleton<RepositoryFactory>();

      #endregion

      #region Websockets & notifications DI

      services.AddWebSocketManager();
      var provider = services.BuildServiceProvider();
      var connectionManager = provider.GetRequiredService<WebSocketConnectionManager>();
      var wsHandler = new ChangeNotificationsMessageHandler(connectionManager);
      services.AddSingleton<ChangeNotificationsMessageHandler>(wsHandler);
      services.AddSingleton<INotifier<Change>>(wsHandler);

      #endregion

      #region Background workers DI

      services.AddTransient<PoloniexChangeMonitor>();
      services.AddTransient<BittrexChangeMonitor>();

      services.AddSingleton<BittrexTimerWorker>();
      services.AddSingleton<PoloniexTimerWorker>();

      #endregion
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
      IServiceProvider serviceProvider)
    {
      var webSocketOptions = new WebSocketOptions()
      {
        KeepAliveInterval = TimeSpan.FromSeconds(120)
      };

      app.UseWebSockets(webSocketOptions);

      app.MapWebSocketManager("/changeNotifications",
        serviceProvider.GetRequiredService<ChangeNotificationsMessageHandler>());

      app.Use(async (context, next) =>
      {
        await next();
        if (context.Response.StatusCode == 404 &&
            !Path.HasExtension(context.Request.Path.Value) &&
            !context.Request.Path.Value.StartsWith("/api/"))
        {
          context.Request.Path = "/index.html";
          await next();
        }
      });
      app.UseMvcWithDefaultRoute();
      app.UseDefaultFiles();
      app.UseStaticFiles();


      loggerFactory.AddConsole();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
    }
  }
}
