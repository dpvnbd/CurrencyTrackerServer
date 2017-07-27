using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.BittrexService.Entities;
using Microsoft.EntityFrameworkCore;


namespace CurrencyTrackerServer.Tests.BittrexService.Entities
{
    internal class BittrexTestContext : DbContext
    {

        public DbSet<TestEntity> TestSet { get; set; }
        public DbSet<CurrencyStateEntity> States { get; set; }
        public DbSet<ChangeHistoryEntryEntity> History { get; set; }


        public BittrexTestContext(DbContextOptions<BittrexTestContext> options) : base(options)
        {
        }
    }
}
