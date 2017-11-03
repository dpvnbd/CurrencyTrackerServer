using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CurrencyTrackerServer.Infrastructure.Concrete.Identity
{
  public class IdentityDbFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
  {
 
    public ApplicationDbContext CreateDbContext(string[] args)
    {

      IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
        .Build();

      var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

      var connectionString = configuration.GetConnectionString("ChangesDb");

      builder.UseSqlServer(connectionString);

      return new ApplicationDbContext(builder.Options);
    }
  }
}