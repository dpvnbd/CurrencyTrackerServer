using CurrencyTrackerServer.ChangeTrackerService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTrackerServer.Tests.ChangeTrackerService.Entities
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
