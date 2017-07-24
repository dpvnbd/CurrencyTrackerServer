using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTrackerServer.Data
{
    public class BittrexContext:DbContext
    {
        public DbSet<BittrexChange> Changes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=bittrex.db");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
