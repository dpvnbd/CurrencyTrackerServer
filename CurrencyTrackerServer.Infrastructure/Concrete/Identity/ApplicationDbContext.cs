using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTrackerServer.Infrastructure.Concrete.Identity
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
      public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
      {
      }

      protected override void OnModelCreating(ModelBuilder builder)
      {
        base.OnModelCreating(builder);
      }
    }
}
