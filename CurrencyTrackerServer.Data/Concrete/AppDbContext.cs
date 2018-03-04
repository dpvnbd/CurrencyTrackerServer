using System.ComponentModel.DataAnnotations.Schema;
using CurrencyTrackerServer.Data.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTrackerServer.Data.Concrete
{
  public class AppDbContext : IdentityDbContext<ApplicationUser>
  {

    public DbSet<CurrencyState> States { get; set; }
    public DbSet<ChangeHistoryEntry> History { get; set; }
    public DbSet<SettingsSerialized> UserSettings { get; set; }
    public DbSet<NoticeEntity> Notices { get; set; }

    public DbSet<StatsCurrencyState> StatsStates { get; set; }
    public DbSet<StatsAverageChangeEntry> AverageChanges { get; set; }


    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
      
      builder.Entity<CurrencyState>()
        .HasKey(c => new { c.Currency, c.UpdateSource, c.UserId });

      builder.Entity<ChangeHistoryEntry>(c =>
      {
        c.HasKey(e => new {e.Id, e.UserId});
        c.Property(e => e.Id).ValueGeneratedOnAdd();
      });

      builder.Entity<SettingsSerialized>()
        .HasKey(c => new { c.UserId, c.Source, c.Destination });

      builder.Entity<NoticeEntity>(n =>
      {
        n.HasKey(e => new { e.Id, e.Source });
        n.Property(e => e.Id).ValueGeneratedOnAdd();
      });

      builder.Entity<StatsCurrencyState>()
        .HasKey(c => new { c.Currency, c.UpdateSource });

      builder.Entity<StatsAverageChangeEntry>()
        .HasKey(c => new { c.UpdateSource, c.Timestamp });
    }
  }
}
