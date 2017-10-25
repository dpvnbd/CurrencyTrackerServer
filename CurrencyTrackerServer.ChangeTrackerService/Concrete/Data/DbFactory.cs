using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.Data
{
  public class DbFactory : IDesignTimeDbContextFactory<ChangeTrackerContext>
  {
 
    public ChangeTrackerContext CreateDbContext(string[] args)
    {

      IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

      var builder = new DbContextOptionsBuilder<ChangeTrackerContext>();

      var connectionString = configuration.GetConnectionString("ChangesDb");

      builder.UseSqlServer(connectionString);

      return new ChangeTrackerContext(builder.Options);
    }
  }
}