using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.BittrexService.Concrete;
using CurrencyTrackerServer.Infrastructure.Abstract;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTrackerServer.Tests.BittrexService.Entities
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
