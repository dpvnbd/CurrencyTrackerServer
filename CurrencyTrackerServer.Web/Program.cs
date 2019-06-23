using System;
using System.IO;
using CurrencyTrackerServer.Web.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;


namespace CurrencyTrackerServer.Web
{
  public class Program
  {
    public static int Main(string[] args)
    {
      Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/log.txt", LogEventLevel.Warning, rollingInterval: RollingInterval.Day)
        .CreateLogger();

      try
      {
        var host = BuildWebHost(args);
        using (var scope = host.Services.CreateScope())
        {
          // place your DB seeding code here
          IdentityAdminSetup.CreateRoles(scope.ServiceProvider);
        }

        Log.Warning("Starting web host");
        host.Run();
        return 0;
      }
      catch (Exception ex)
      {
        Log.Fatal(ex, "Host terminated unexpectedly");
        return 1;
      }
      finally
      {
        Log.Warning("Host is shut down");
        Log.CloseAndFlush();
      }
    }

    public static IWebHost BuildWebHost(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
               .UseContentRoot(Directory.GetCurrentDirectory())
               .UseStartup<Startup>()
               .UseSerilog()
               .Build();
  }
}
