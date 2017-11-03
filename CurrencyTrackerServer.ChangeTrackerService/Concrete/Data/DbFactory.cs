using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.Data
{
  public class DbFactory : IDesignTimeDbContextFactory<ChangeTrackerContext>
  {


 
    public ChangeTrackerContext CreateDbContext(string[] args)
    {

      IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
        .Build();

      var builder = new DbContextOptionsBuilder<ChangeTrackerContext>();

      var connectionString = configuration.GetConnectionString("ChangesDb");

      builder.UseSqlServer(connectionString);

      return new ChangeTrackerContext(builder.Options);
    }
  }
}