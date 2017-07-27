
using CurrencyTrackerServer.BittrexService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTrackerServer.BittrexService.Concrete
{
    public class BittrexContext : DbContext
    {
        public DbSet<CurrencyStateEntity> States { get; set; }
        public DbSet<ChangeHistoryEntryEntity> History { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=bittrex.service.db");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
