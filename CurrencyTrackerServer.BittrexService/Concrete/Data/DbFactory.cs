using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.Data
{
    public class DbFactory : IDbContextFactory<ChangeTrackerContext>
    {
        public ChangeTrackerContext Create()
        {
            var environmentName =
                Environment.GetEnvironmentVariable(
                    "Hosting:Environment");

            var basePath = AppContext.BaseDirectory;

            return Create(basePath, environmentName);
        }

        public ChangeTrackerContext Create(DbContextFactoryOptions options)
        {
            return Create(options.ContentRootPath, options.EnvironmentName);
        }

        private ChangeTrackerContext Create(string basePath, string environmentName)
        {
            var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", true);

            var config = builder.Build();

            var connstr = config.GetConnectionString("ChangesDb");

            if (String.IsNullOrWhiteSpace(connstr) == true)
            {
                throw new InvalidOperationException(
                    "Could not find a connection string named 'ChangesDb'.");
            }
            else
            {
                return Create(connstr);
            }
        }

        private ChangeTrackerContext Create(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException(
                    $"{nameof(connectionString)} is null or empty.",
                    nameof(connectionString));

            var optionsBuilder =
                new DbContextOptionsBuilder<ChangeTrackerContext>();

            optionsBuilder.UseSqlServer(connectionString);

            return new ChangeTrackerContext(optionsBuilder.Options);
        }
    }
}