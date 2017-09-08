using CurrencyTrackerServer.ChangeTrackerService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.Data
{
    public class ChangeTrackerContext : DbContext
    {
        public DbSet<CurrencyStateEntity> States { get; set; }
        public DbSet<ChangeHistoryEntryEntity> History { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=change.service.db");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CurrencyStateEntity>()
                .HasKey(c => new { c.Currency, c.ChangeSource });

            modelBuilder.Entity<ChangeHistoryEntryEntity>()
                .HasKey(c => new { c.Currency, c.ChangeSource });
        }
    }
}
