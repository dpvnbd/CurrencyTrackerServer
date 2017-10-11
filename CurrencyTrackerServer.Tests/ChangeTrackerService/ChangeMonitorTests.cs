using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Concrete;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Data;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;
using CurrencyTrackerServer.Tests.ChangeTrackerService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CurrencyTrackerServer.Tests.ChangeTrackerService
{
    [TestClass]
    public class ChangeMonitorTests
    {
        private static IRepositoryFactory _repoFactory;

        [ClassInitialize]
        public static void Init(TestContext ctx)
        {
            var dbFactory = new TestDbContextFactory();
            _repoFactory = new RepositoryFactory(dbFactory, null);
        }

        [TestInitialize]
        public async Task Setup()
        {
            using (var repo = _repoFactory.Create<CurrencyStateEntity>())
            {
                await repo.DeleteAll();
            }

            using (var repo = _repoFactory.Create<ChangeHistoryEntryEntity>())
            {
                await repo.DeleteAll();
            }
        }

        [TestMethod]
        public async Task TestStateReset()
        {
            //Arrange

            var dataSourceMock = new Mock<IDataSource<IEnumerable<CurrencyChangeApiData>>>();
            dataSourceMock.Setup(m => m.Source).Returns(ChangeSource.None);

            using (var stateRepo = _repoFactory.Create<CurrencyStateEntity>())
            {
                await stateRepo.Add(new CurrencyStateEntity
                {
                    Currency = "BTC",
                    LastChangeTime = DateTime.Now,
                    Threshold = 45
                });
            }
            var monitor = new ChangeMonitor(dataSourceMock.Object, _repoFactory,
                new TestSettingsProvider(new ChangeSettings()));
            //Act
            await monitor.ResetAll();

            //Assert
            using (var stateRepo = _repoFactory.Create<CurrencyStateEntity>())
            {
                var state = stateRepo.GetAll().FirstOrDefault();
                Assert.IsNull(state);
            }

            using (var historyRepo = _repoFactory.Create<ChangeHistoryEntryEntity>())
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
            var dataSourceMock = new Mock<IDataSource<IEnumerable<CurrencyChangeApiData>>>();
            var apiChange = new CurrencyChangeApiData {Currency = "BTC", CurrentBid = 100, PreviousDayBid = 50};
            var apiData = new List<CurrencyChangeApiData>
            {
                apiChange,
                new CurrencyChangeApiData {Currency = "NoChange", CurrentBid = 10, PreviousDayBid = 9}
            };
            dataSourceMock.Setup(m => m.GetData()).ReturnsAsync(apiData);

            var settings = new ChangeSettings()
            {
                Percentage = 45,
                MultipleChanges = false
            };

            var monitor =
                new ChangeMonitor(dataSourceMock.Object, _repoFactory, new TestSettingsProvider(settings));
            //Act
            var changes = (await monitor.GetChanges()).ToList();
            var change = changes.FirstOrDefault();

            List<CurrencyStateEntity> states;
            List<ChangeHistoryEntryEntity> history;

            using (var stateRepo = _repoFactory.Create<CurrencyStateEntity>())
            {
                states = new List<CurrencyStateEntity>(stateRepo.GetAll());
            }

            using (var historyRepo = _repoFactory.Create<ChangeHistoryEntryEntity>())
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
            var dataSourceMock = new Mock<IDataSource<IEnumerable<CurrencyChangeApiData>>>();
            var apiChange = new CurrencyChangeApiData {Currency = "BTC", CurrentBid = 100, PreviousDayBid = 50};
            var apiData = new List<CurrencyChangeApiData>
            {
                apiChange,
                new CurrencyChangeApiData {Currency = "OneChange", CurrentBid = 100, PreviousDayBid = 9}
            };
            dataSourceMock.Setup(m => m.GetData()).ReturnsAsync(apiData);

            using (var repo = _repoFactory.Create<CurrencyStateEntity>())
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
            var settings = new ChangeSettings()
            {
                Percentage = 45,
                MultipleChangesSpanMinutes = 30,
                MultipleChanges = true
            };

            var monitor =
                new ChangeMonitor(dataSourceMock.Object, _repoFactory, new TestSettingsProvider(settings));

            var changes = (await monitor.GetChanges()).ToList();

            var change = changes.FirstOrDefault();

            List<CurrencyStateEntity> states;
            List<ChangeHistoryEntryEntity> history;

            using (var stateRepo = _repoFactory.Create<CurrencyStateEntity>())
            {
                states = new List<CurrencyStateEntity>(stateRepo.GetAll());
            }

            using (var historyRepo = _repoFactory.Create<ChangeHistoryEntryEntity>())
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

        [TestMethod]
        public async Task TestMultipleQuickChanges()
        {
            //Arrange
            var dataSourceMock = new Mock<IDataSource<IEnumerable<CurrencyChangeApiData>>>();
            var apiChange = new CurrencyChangeApiData {Currency = "MUSIC", CurrentBid = 105, PreviousDayBid = 100};
            var apiChange1 = new CurrencyChangeApiData {Currency = "MUSIC", CurrentBid = 108, PreviousDayBid = 100};
            var apiChange2 = new CurrencyChangeApiData {Currency = "MUSIC", CurrentBid = 112, PreviousDayBid = 100};
            var apiData = new List<CurrencyChangeApiData>
            {
                apiChange,
            };
            dataSourceMock.Setup(m => m.GetData()).ReturnsAsync(apiData);



            var settings = new ChangeSettings()
            {
                Percentage = 5,
                MultipleChangesSpanMinutes = 2,
                MultipleChanges = true
            };

            var monitor =
                new ChangeMonitor(dataSourceMock.Object, _repoFactory, new TestSettingsProvider(settings));

            await monitor.GetChanges();

            apiData[0] = apiChange1;
            await monitor.GetChanges();

            apiData[0] = apiChange2;
            await monitor.GetChanges();
            
            List<ChangeHistoryEntryEntity> history;
            
            using (var historyRepo = _repoFactory.Create<ChangeHistoryEntryEntity>())
            {
                history = new List<ChangeHistoryEntryEntity>(historyRepo.GetAll());
            }

            Assert.AreEqual(1, history.Count);
        }
    }
}