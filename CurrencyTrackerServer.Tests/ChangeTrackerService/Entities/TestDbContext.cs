using CurrencyTrackerServer.ChangeTrackerService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTrackerServer.Tests.ChangeTrackerService.Entities
{
    internal class TestDbContext : DbContext
    {

        public DbSet<TestEntity> TestSet { get; set; }


        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }
    }
}
