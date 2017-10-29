using CurrencyTrackerServer.ChangeTrackerService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.Data
{
    public class ChangeTrackerContext : DbContext
    {
        public DbSet<CurrencyStateEntity> States { get; set; }
        public DbSet<ChangeHistoryEntryEntity> History { get; set; }

        

        public ChangeTrackerContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CurrencyStateEntity>()
                .HasKey(c => new {c.Currency, c.ChangeSource});
          
        }
    }
}