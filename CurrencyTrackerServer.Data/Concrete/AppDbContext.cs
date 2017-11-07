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


    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.Entity<CurrencyState>()
        .HasKey(c => new { c.Currency, c.UpdateSource });


      builder.Entity<SettingsSerialized>()
        .HasKey(c => new { c.UserId, c.Source, c.Destination });


    }


  }
}
