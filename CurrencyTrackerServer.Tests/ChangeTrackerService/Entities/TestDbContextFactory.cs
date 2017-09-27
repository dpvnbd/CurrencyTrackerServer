using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CurrencyTrackerServer.Tests.ChangeTrackerService.Entities
{
    internal class TestDbContextFactory:IDbContextFactory<ChangeTrackerContext>
    {
        public ChangeTrackerContext Create(DbContextFactoryOptions options)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ChangeTrackerContext>()
                .UseInMemoryDatabase(databaseName: "TestMontor");
            
            return new ChangeTrackerContext(optionsBuilder.Options);
        }
    }
}
