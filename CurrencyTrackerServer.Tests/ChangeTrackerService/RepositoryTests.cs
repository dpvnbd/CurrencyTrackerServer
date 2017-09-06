using System.Linq;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Concrete;
using CurrencyTrackerServer.Tests.ChangeTrackerService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CurrencyTrackerServer.Tests.ChangeTrackerService
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
                await service.Add(constituent);
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