using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CurrencyTrackerServer.Tests.BittrexService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CurrencyTrackerServer.BittrexService.Entities;
using System.Threading.Tasks;
using CurrencyTrackerServer.BittrexService.Concrete;

namespace CurrencyTrackerServer.Tests.BittrexService
{
    [TestClass]
    public class RepositoryTests
    {
        [TestMethod]
        public async Task TestAdd()
        {
            var options = new DbContextOptionsBuilder<BittrexTestContext>()
                .UseInMemoryDatabase(databaseName: "TestCRUD")
                .Options;

            // Run the test against one instance of the context
            using (var context = new BittrexTestContext(options))
            {
                var constituent = new TestEntity
                {
                    Currency = "BTC",
                    Percentage = 10.12
                };
                var service = new Repository<TestEntity>(context);
                service.Add(constituent);
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new BittrexTestContext(options))
            {
                var entity = context.TestSet.FirstOrDefault();
                Assert.AreEqual(1, context.TestSet.Count());
                Assert.AreEqual("BTC", entity.Currency);
                Assert.IsTrue(entity.Percentage == 10.12);

                var service = new Repository<TestEntity>(context);
                entity.Percentage = 42;
                await service.Update(entity);
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new BittrexTestContext(options))
            {
                var entity = context.TestSet.FirstOrDefault();

                Assert.AreEqual(1, context.TestSet.Count());
                Assert.AreEqual("BTC", entity.Currency);
                Assert.IsTrue(entity.Percentage == 42);

                var service = new Repository<TestEntity>(context);
                await service.Delete(entity);
            }

            //use separate context to verify delete
            using (var context = new BittrexTestContext(options))
            {
                Assert.AreEqual(0, context.TestSet.Count());
            }
        }
    }
}