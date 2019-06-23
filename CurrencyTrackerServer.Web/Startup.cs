using System;
using System.IO;
using System.Text;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Bittrex;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Poloniex;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.ChangeTrackerService.Concrete;
using CurrencyTrackerServer.Data.Concrete;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Changes;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;
using CurrencyTrackerServer.Infrastructure.Entities.Data;
using CurrencyTrackerServer.Infrastructure.Entities.Price;
using CurrencyTrackerServer.NoticesService.Concrete;
using CurrencyTrackerServer.NoticesService.Entities;
using CurrencyTrackerServer.PriceService.Concrete;
using CurrencyTrackerServer.PriceService.Concrete.Bittrex;
using CurrencyTrackerServer.PriceService.Concrete.Poloniex;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CurrencyTrackerServer.ReminderService;
using CurrencyTrackerServer.Web.Infrastructure.Concrete;
using CurrencyTrackerServer.Web.Infrastructure.Concrete.MultipleUsers;
using CurrencyTrackerServer.Web.Infrastructure.Websockets;
using CurrencyTrackerServer.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Binance;
using CurrencyTrackerServer.PriceService.Concrete.Binance;
using Microsoft.AspNetCore.HttpOverrides;

namespace CurrencyTrackerServer.Web
{
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration config)
    {
      Configuration = config;
      Log.Warning("Environment variable: " + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
          // Password settings
          options.Password.RequireDigit = false;
          options.Password.RequiredLength = 4;
          options.Password.RequireNonAlphanumeric = false;
          options.Password.RequireUppercase = false;
          options.Password.RequireLowercase = false;
          options.Password.RequiredUniqueChars = 1;

          options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();


      services.AddAuthentication(options =>
      {
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
      })
        .AddJwtBearer(cfg =>
        {
          cfg.SecurityTokenValidators.Clear();
          cfg.SecurityTokenValidators.Add(new JwtSecurityTokenHandler
          {
            // Disable the built-in JWT claims mapping feature.
            InboundClaimTypeMap = new Dictionary<string, string>()
          });

          cfg.RequireHttpsMetadata = false;
          cfg.SaveToken = true;

          cfg.TokenValidationParameters = new TokenValidationParameters()
          {
            ValidIssuer = Configuration["TrackerTokens:Issuer"],
            ValidAudience = Configuration["TrackerTokens:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TrackerTokens:Key"]))
          };


        });


      services.AddMvc();
      services.AddWebSocketManager();

      services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(Configuration.GetConnectionString("ChangesDb")));

      #region Persistence DI
      services.AddSingleton<IDesignTimeDbContextFactory<AppDbContext>, AppDbContextFactory>();
      services.AddSingleton<IRepositoryFactory, RepositoryFactory>();
      services.AddSingleton<ISettingsProvider, DbSettingsProvider>();

      #endregion

      #region Websockets & notifications DI
      var provider = services.BuildServiceProvider();

      var changesConnectionManager = provider.GetRequiredService<WebSocketConnectionManager>();
      var changeHandler = new NotificationsMessageHandler(changesConnectionManager);
      services.AddSingleton<NotificationsMessageHandler>(changeHandler);
      services.AddSingleton<INotifier>(changeHandler);

      #endregion

      #region ChangeTracker DI

      //services.AddTransient<PoloniexChangeMonitor>();
      //services.AddTransient<BittrexChangeMonitor>();

      services.AddSingleton<IChangesStatsService<CurrencyChangeApiData>, ChangesStatsService>();

      services.AddSingleton<BittrexTimerWorker>();
      services.AddSingleton<PoloniexTimerWorker>();
      services.AddSingleton<BinanceTimerWorker>();

      services.AddSingleton<BittrexApiDataSource>();
      services.AddSingleton<PoloniexApiDataSource>();
      services.AddSingleton<BinanceApiDataSource>();
      #endregion

      #region PriceTracker DI

      //services.AddTransient<BittrexPriceMonitor>();
      //services.AddTransient<PoloniexPriceMonitor>();

      services.AddSingleton<BittrexPriceTimerWorker>();
      services.AddSingleton<PoloniexPriceTimerWorker>();
      services.AddSingleton<BinancePriceTimerWorker>();

      services.AddSingleton<BittrexPriceDataSource>();
      services.AddSingleton<PoloniexPriceDataSource>();
      services.AddSingleton<BinancePriceDataSource>();

      #endregion

      #region Reminder
      services.AddSingleton<ReminderTimerWorker>();
      #endregion

      services.Configure<SmtpSettings>(Configuration.GetSection("SmtpSettings"));
      services.Configure<TwitterSettings>(Configuration.GetSection("TwitterKeys"));
      services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

      services.AddSingleton<IMessageNotifier, EmailNotifier>();

      services.AddSingleton<DefaultNoticesTimerWorker>();
      services.AddSingleton<PoloniexTwitterNoticesDataSource>();
      services.AddSingleton<PoloniexSiteNoticesDataSource>();
      services.AddSingleton<PoloniexNoticesMonitor>();

      services.AddSingleton<UserContainerFactory>();
      services.AddSingleton<UserContainersManager>();

      services.AddCors(options =>
      {
        options.AddPolicy("AllowAllOrigins",
            builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
      });

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env,
      IServiceProvider serviceProvider, IApplicationLifetime applicationLifetime)
    {
      app.UseForwardedHeaders(new ForwardedHeadersOptions
      {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
      });

      var webSocketOptions = new WebSocketOptions()
      {
        KeepAliveInterval = TimeSpan.FromSeconds(120)
      };

      app.UseWebSockets(webSocketOptions);

      app.MapWebSocketManager("/notifications",
        serviceProvider.GetRequiredService<NotificationsMessageHandler>());

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
      app.UseAuthentication();
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
