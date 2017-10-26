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
using CurrencyTrackerServer.Infrastructure.Entities.Price;
using CurrencyTrackerServer.PriceService.Concrete;
using CurrencyTrackerServer.PriceService.Concrete.Bittrex;
using CurrencyTrackerServer.PriceService.Concrete.Poloniex;
using CurrencyTrackerServer.Web.Infrastructure;
using CurrencyTrackerServer.Web.Infrastructure.Concrete;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CurrencyTrackerServer.ReminderService;
using Microsoft.EntityFrameworkCore.Design;
using Serilog;

namespace CurrencyTrackerServer.Web
{
  public class Startup
  {
    private IHostingEnvironment _env;
    public IConfiguration Configuration { get; }

    public Startup(IHostingEnvironment env, IConfiguration configuration)
    {
      Configuration = configuration;

      _env = env;

      //var builder = new ConfigurationBuilder()
      //  .SetBasePath(env.ContentRootPath)
      //  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
      //  .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
      //  .AddEnvironmentVariables();
      //Configuration = builder.Build();
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc();
      services.AddWebSocketManager();

      var provider = services.BuildServiceProvider();
      var changesConnectionManager = provider.GetRequiredService<WebSocketConnectionManager>();

      #region Persistence DI

      services.AddDbContext<ChangeTrackerContext>(options => options
        .UseSqlServer(Configuration.GetConnectionString("ChangesDb")), ServiceLifetime.Transient);

      services.AddSingleton<IDesignTimeDbContextFactory<DbContext>, DbFactory>();
      services.AddSingleton<IRepositoryFactory, RepositoryFactory>();

      #endregion


      #region Websockets & notifications DI

      var changeHandler = new NotificationsMessageHandler<Change>(changesConnectionManager);
      services.AddSingleton<NotificationsMessageHandler<Change>>(changeHandler);
      services.AddSingleton<INotifier<Change>>(changeHandler);

      #endregion

      #region Background workers DI

      services.AddSingleton<ISettingsProvider<ChangeSettings>, ChangeSettingsProvider>();

      services.AddTransient<PoloniexChangeMonitor>();
      services.AddTransient<BittrexChangeMonitor>();

      services.AddSingleton<BittrexTimerWorker>();
      services.AddSingleton<PoloniexTimerWorker>();

      #endregion

      #region PriceTracker DI

      services.AddSingleton<ISettingsProvider<PriceSettings>, PriceSettingsProvider>();
      services.AddSingleton<BittrexPriceDataSource>();
      services.AddSingleton<BittrexPriceMonitor>();
      services.AddSingleton<BittrexPriceTimerWorker>();

      services.AddSingleton<PoloniexPriceDataSource>();
      services.AddSingleton<PoloniexPriceMonitor>();
      services.AddSingleton<PoloniexPriceTimerWorker>();

      var priceConnectionManager = provider.GetRequiredService<WebSocketConnectionManager>();

      var priceHandler = new NotificationsMessageHandler<Price>(priceConnectionManager);
      services.AddSingleton<NotificationsMessageHandler<Price>>(priceHandler);
      services.AddSingleton<INotifier<Price>>(priceHandler);
      #endregion

      #region Reminder
      var reminderConnectionManager = provider.GetRequiredService<WebSocketConnectionManager>();
      var reminderHandler = new NotificationsMessageHandler<Reminder>(reminderConnectionManager);
      services.AddSingleton<NotificationsMessageHandler<Reminder>>(reminderHandler);
      services.AddSingleton<INotifier<Reminder>>(reminderHandler);

      services.AddSingleton<ReminderTimerWorker>();

      #endregion
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env,
      IServiceProvider serviceProvider, IApplicationLifetime applicationLifetime)
    {
      var webSocketOptions = new WebSocketOptions()
      {
        KeepAliveInterval = TimeSpan.FromSeconds(120)
      };

      app.UseWebSockets(webSocketOptions);

      app.MapWebSocketManager("/changeNotifications",
        serviceProvider.GetRequiredService<NotificationsMessageHandler<Change>>());

      app.MapWebSocketManager("/priceNotifications",
        serviceProvider.GetRequiredService<NotificationsMessageHandler<Price>>());

      app.MapWebSocketManager("/reminderNotifications",
        serviceProvider.GetRequiredService<NotificationsMessageHandler<Reminder>>());

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

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      applicationLifetime.ApplicationStarted.Register(AppStarting);
      applicationLifetime.ApplicationStopping.Register(AppStopping);
    }

    private void AppStopping()
    {
        Log.Warning("Stopping App");
    }

    private void AppStarting()
    {
        Log.Warning("Starting App");
    }
  }
}
