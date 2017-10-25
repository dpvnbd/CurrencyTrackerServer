using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CurrencyTrackerServer.Tests.ChangeTrackerService.Entities
{
  internal class TestDbContextFactory : IDesignTimeDbContextFactory<ChangeTrackerContext>
  {


    public ChangeTrackerContext CreateDbContext(string[] args)
    {
      var optionsBuilder = new DbContextOptionsBuilder<ChangeTrackerContext>()
          .UseInMemoryDatabase(databaseName: "TestMontor");

      return new ChangeTrackerContext(optionsBuilder.Options);
    }
  }
}
