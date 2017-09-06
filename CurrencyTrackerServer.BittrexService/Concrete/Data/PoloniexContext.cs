using CurrencyTrackerServer.ChangeTrackerService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.Data
{
    public class PoloniexContext : DbContext
    {
        public DbSet<CurrencyStateEntity> States { get; set; }
        public DbSet<ChangeHistoryEntryEntity> History { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=poloniex.service.db");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
