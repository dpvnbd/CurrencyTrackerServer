using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.Data
{
    class ChangeDbFactory : IDbContextFactory<ChangeTrackerContext>
    {
    

        public ChangeTrackerContext Create(DbContextFactoryOptions options)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ChangeTrackerContext>();
            optionsBuilder.UseSqlServer("Server = (localdb)\\MSSQLLocalDB; Database = Changes; Trusted_Connection = True; MultipleActiveResultSets = true");

            return new ChangeTrackerContext(optionsBuilder.Options);
        }
    }
}
