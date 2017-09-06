using CurrencyTrackerServer.ChangeTrackerService.Concrete;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTrackerServer.Tests.ChangeTrackerService.Entities
{
    class TestRepository<T>:Repository<T> where T:class
    {
        public TestRepository()
        {
            var options = new DbContextOptionsBuilder<BittrexTestContext>()
                .UseInMemoryDatabase(databaseName: "TestMonotor")
                .Options;
            
            Context = new BittrexTestContext(options);
        }
    }
    
}
