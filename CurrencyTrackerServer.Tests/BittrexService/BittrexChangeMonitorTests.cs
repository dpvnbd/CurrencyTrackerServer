using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CurrencyTrackerServer.Tests.BittrexService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Concrete;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using Moq;

namespace CurrencyTrackerServer.Tests.BittrexService
{
    [TestClass]
    public class BittrexChangeMonitorTests
    {
        private ChangeMonitor<TestRepository<CurrencyStateEntity>, TestRepository<ChangeHistoryEntryEntity>>
            _monitor;

        [TestInitialize]
        public async Task Setup()
        {
            var datasourceMock = new Mock<IDataSource<List<CurrencyChangeApiData>>>();
            _monitor = new ChangeMonitor<TestRepository<CurrencyStateEntity>,
                TestRepository<ChangeHistoryEntryEntity>>(datasourceMock.Object);
            using (var stateRepo = new TestRepository<CurrencyStateEntity>())
            {
                await stateRepo.DeleteAll();
            }

            using (var historyRepo = new TestRepository<ChangeHistoryEntryEntity>())
            {
                await historyRepo.DeleteAll();
            }
        }

        [TestMethod]
        public async Task TestStateReset()
        {
            //Arrange
            using (var stateRepo = new TestRepository<CurrencyStateEntity>())
            {
                await stateRepo.Add(new CurrencyStateEntity
                {
                    Currency = "BTC",
                    LastChangeTime = DateTime.Now,
                    Threshold = 45
                });
            }

            //Act
            await _monitor.ResetAll();

            //Assert
            using (var stateRepo = new TestRepository<CurrencyStateEntity>())
            {
                var state = stateRepo.GetAll().FirstOrDefault();
                Assert.IsNull(state);
            }

            using (var historyRepo = new TestRepository<ChangeHistoryEntryEntity>())
            {
                var historyEntry = historyRepo.GetAll().FirstOrDefault();
                Assert.IsNotNull(historyEntry);
                Assert.IsTrue(historyEntry.Type == ChangeType.Info);
            }
        }

        [TestMethod]
        public async Task TestSimpleChange()
        {
            //Arrange
            var dataSourceMock = new Mock<IDataSource<List<CurrencyChangeApiData>>>();
            var apiChange = new CurrencyChangeApiData {Currency = "BTC", CurrentBid = 100, PreviousDayBid = 50};
            var apiData = new List<CurrencyChangeApiData>
            {
                apiChange,
                new CurrencyChangeApiData {Currency = "NoChange", CurrentBid = 10, PreviousDayBid = 9}
            };
            dataSourceMock.Setup(m => m.GetData()).ReturnsAsync(apiData);

            _monitor =
                new ChangeMonitor<TestRepository<CurrencyStateEntity>, TestRepository<ChangeHistoryEntryEntity>>(
                    dataSourceMock.Object);
            //Act
            var changes = await _monitor.GetChanges(45, TimeSpan.MinValue, false);
            var change = changes.FirstOrDefault();

            List<CurrencyStateEntity> states;
            List<ChangeHistoryEntryEntity> history;

            using (var stateRepo = new TestRepository<CurrencyStateEntity>())
            {
                states = new List<CurrencyStateEntity>(stateRepo.GetAll());
            }

            using (var historyRepo = new TestRepository<ChangeHistoryEntryEntity>())
            {
                history = new List<ChangeHistoryEntryEntity>(historyRepo.GetAll());
            }

            var state = states.FirstOrDefault();
            var historyEntry = history.FirstOrDefault();

            //Assert
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(1, states.Count);
            Assert.AreEqual(1, history.Count);

            Assert.IsNotNull(change);
            Assert.AreEqual(apiChange.Currency, change.Currency);
            Assert.AreEqual(100, change.Percentage);
            Assert.AreEqual(90, change.Threshold);
            Assert.AreEqual(ChangeType.Currency, change.Type);
            Assert.IsTrue(change.Time > DateTime.Now - TimeSpan.FromMinutes(10));

            Assert.IsNotNull(state);
            Assert.AreEqual(change.Currency, state.Currency);
            Assert.AreEqual(90, state.Threshold);
            Assert.IsTrue(state.LastChangeTime > DateTime.Now - TimeSpan.FromMinutes(10));
            Assert.IsTrue(state.Created > DateTime.Now - TimeSpan.FromMinutes(10));

            Assert.IsNotNull(historyEntry);
            Assert.AreEqual(change.Currency, historyEntry.Currency);
            Assert.AreEqual(100, historyEntry.Percentage);
            Assert.AreEqual(ChangeType.Currency, historyEntry.Type);
            Assert.IsTrue(historyEntry.Time > DateTime.Now - TimeSpan.FromMinutes(10));
        }

        [TestMethod]
        public async Task TestMultipleChanges()
        {
            //Arrange
            var dataSourceMock = new Mock<IDataSource<List<CurrencyChangeApiData>>>();
            var apiChange = new CurrencyChangeApiData {Currency = "BTC", CurrentBid = 100, PreviousDayBid = 50};
            var apiData = new List<CurrencyChangeApiData>
            {
                apiChange,
                new CurrencyChangeApiData {Currency = "OneChange", CurrentBid = 100, PreviousDayBid = 9}
            };
            dataSourceMock.Setup(m => m.GetData()).ReturnsAsync(apiData);

            using (var repo = new TestRepository<CurrencyStateEntity>())
            {
                await repo.Add(new CurrencyStateEntity
                {
                    Currency = "BTC",
                    LastChangeTime = DateTime.Now - TimeSpan.FromMinutes(20),
                    Threshold = 10,
                    Created = DateTime.MinValue
                });

                await repo.Add(new CurrencyStateEntity
                {
                    Currency = "NotChanged",
                    LastChangeTime = DateTime.Now,
                    Threshold = 10
                });

                await repo.Add(new CurrencyStateEntity
                {
                    Currency = "Expired",
                    LastChangeTime = DateTime.MinValue,
                    Threshold = 10
                });
            }

            _monitor =
                new ChangeMonitor<TestRepository<CurrencyStateEntity>, TestRepository<ChangeHistoryEntryEntity>>(
                    dataSourceMock.Object);

            var changes = await _monitor.GetChanges(45, TimeSpan.FromMinutes(30), true);

            var change = changes.FirstOrDefault();

            List<CurrencyStateEntity> states;
            List<ChangeHistoryEntryEntity> history;

            using (var stateRepo = new TestRepository<CurrencyStateEntity>())
            {
                states = new List<CurrencyStateEntity>(stateRepo.GetAll());
            }

            using (var historyRepo = new TestRepository<ChangeHistoryEntryEntity>())
            {
                history = new List<ChangeHistoryEntryEntity>(historyRepo.GetAll());
            }

            var state = states.FirstOrDefault();
            var historyEntry = history.FirstOrDefault();

            //Assert
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(4, states.Count);
            Assert.AreEqual(1, history.Count);

            Assert.IsNotNull(change);
            Assert.AreEqual(apiChange.Currency, change.Currency);
            Assert.AreEqual(100, change.Percentage);
            Assert.AreEqual(90, change.Threshold);
            Assert.AreEqual(ChangeType.Currency, change.Type);
            Assert.IsTrue(change.Time > DateTime.Now - TimeSpan.FromMinutes(10));

            Assert.IsNotNull(state);
            Assert.AreEqual(change.Currency, state.Currency);
            Assert.AreEqual(90, state.Threshold);
            Assert.IsTrue(state.LastChangeTime > DateTime.Now - TimeSpan.FromMinutes(10));
            Assert.AreEqual(DateTime.MinValue, state.Created);

            Assert.IsNotNull(historyEntry);
            Assert.AreEqual(change.Currency, historyEntry.Currency);
            Assert.AreEqual(100, historyEntry.Percentage);
            Assert.AreEqual(ChangeType.Currency, historyEntry.Type);
            Assert.IsTrue(historyEntry.Time > DateTime.Now - TimeSpan.FromMinutes(10));
        }
    }
}