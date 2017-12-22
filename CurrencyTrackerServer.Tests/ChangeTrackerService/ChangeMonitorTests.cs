//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using CurrencyTrackerServer.ChangeTrackerService.Concrete;
//using CurrencyTrackerServer.ChangeTrackerService.Concrete.Data;
//using CurrencyTrackerServer.ChangeTrackerService.Entities;
//using CurrencyTrackerServer.Data.Concrete;
//using CurrencyTrackerServer.Infrastructure.Abstract;
//using CurrencyTrackerServer.Infrastructure.Abstract.Data;
//using CurrencyTrackerServer.Infrastructure.Entities;
//using CurrencyTrackerServer.Infrastructure.Entities.Changes;
//using CurrencyTrackerServer.Infrastructure.Entities.Data;
//using CurrencyTrackerServer.Tests.ChangeTrackerService.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;

//namespace CurrencyTrackerServer.Tests.ChangeTrackerService
//{
//    [TestClass]
//    public class ChangeMonitorTests
//    {
//        private static IRepositoryFactory _repoFactory;

//        [ClassInitialize]
//        public static void Init(TestContext ctx)
//        {
//            var dbFactory = new TestDbContextFactory();
//            _repoFactory = new RepositoryFactory(dbFactory);
//        }

//        [TestInitialize]
//        public async Task Setup()
//        {
//            using (var repo = _repoFactory.Create<CurrencyState>())
//            {
//                await repo.DeleteAll();
//            }

//            using (var repo = _repoFactory.Create<ChangeHistoryEntry>())
//            {
//                await repo.DeleteAll();
//            }
//        }

//        //[TestMethod]
//        //public async Task TestStateReset()
//        //{
//        //    //Arrange

//        //    var dataSourceMock = new Mock<IDataSource<IEnumerable<CurrencyChangeApiData>>>();
//        //    dataSourceMock.Setup(m => m.Source).Returns(UpdateSource.None);

//        //    using (var stateRepo = _repoFactory.Create<CurrencyState>())
//        //    {
//        //        await stateRepo.Add(new CurrencyState
//        //        {
//        //            Currency = "BTC",
//        //            LastChangeTime = DateTime.Now,
//        //            Threshold = 45
//        //        });
//        //    }
//        //    var monitor = new ChangeMonitor(dataSourceMock.Object, _repoFactory,
//        //        new TestSettingsProvider(new ChangeSettings()));
//        //    //Act
//        //    await monitor.ResetAll();

//        //    //Assert
//        //    using (var stateRepo = _repoFactory.Create<CurrencyState>())
//        //    {
//        //        var state = stateRepo.GetAll().FirstOrDefault();
//        //        Assert.IsNull(state);
//        //    }

//        //    using (var historyRepo = _repoFactory.Create<ChangeHistoryEntry>())
//        //    {
//        //        var historyEntry = historyRepo.GetAll().FirstOrDefault();
//        //        Assert.IsNotNull(historyEntry);
//        //        Assert.IsTrue(historyEntry.Type == UpdateType.Info);
//        //    }
//        //}

//        [TestMethod]
//        public async Task TestSimpleChange()
//        {
//            //Arrange
//            var dataSourceMock = new Mock<IDataSource<IEnumerable<CurrencyChangeApiData>>>();
//            var apiChange = new CurrencyChangeApiData { Currency = "BTC", CurrentBid = 100, PreviousDayBid = 50 };
//            var apiData = new List<CurrencyChangeApiData>
//            {
//                apiChange,
//                new CurrencyChangeApiData {Currency = "NoChange", CurrentBid = 10, PreviousDayBid = 9}
//            };
//            dataSourceMock.Setup(m => m.GetData()).ReturnsAsync(apiData);

//            var settings = new ChangeSettings()
//            {
//                Percentage = 45,
//                MultipleChanges = false
//            };

//            var monitor =
//                new ChangeMonitor(dataSourceMock.Object, _repoFactory, new TestSettingsProvider(settings));
//            //Act
//            var changes = (await monitor.CheckChanges()).ToList();
//            var change = changes.FirstOrDefault();

//            List<CurrencyState> states;
//            List<ChangeHistoryEntry> history;

//            using (var stateRepo = _repoFactory.Create<CurrencyState>())
//            {
//                states = new List<CurrencyState>(stateRepo.GetAll());
//            }

//            using (var historyRepo = _repoFactory.Create<ChangeHistoryEntry>())
//            {
//                history = new List<ChangeHistoryEntry>(historyRepo.GetAll());
//            }

//            var state = states.FirstOrDefault();
//            var historyEntry = history.FirstOrDefault();

//            //Assert
//            Assert.AreEqual(1, changes.Count);
//            Assert.AreEqual(1, states.Count);
//            Assert.AreEqual(1, history.Count);

//            Assert.IsNotNull(change);
//            Assert.AreEqual(apiChange.Currency, change.Currency);
//            Assert.AreEqual(100, change.Percentage);
//            Assert.AreEqual(90, change.Threshold);
//            Assert.AreEqual(UpdateType.Currency, change.Type);
//            Assert.IsTrue(change.Time > DateTime.Now - TimeSpan.FromMinutes(10));

//            Assert.IsNotNull(state);
//            Assert.AreEqual(change.Currency, state.Currency);
//            Assert.AreEqual(90, state.Threshold);
//            Assert.IsTrue(state.LastChangeTime > DateTime.Now - TimeSpan.FromMinutes(10));
//            Assert.IsTrue(state.Created > DateTime.Now - TimeSpan.FromMinutes(10));

//            Assert.IsNotNull(historyEntry);
//            Assert.AreEqual(change.Currency, historyEntry.Currency);
//            Assert.AreEqual(100, historyEntry.Percentage);
//            Assert.AreEqual(UpdateType.Currency, historyEntry.Type);
//            Assert.IsTrue(historyEntry.Time > DateTime.Now - TimeSpan.FromMinutes(10));
//        }

//        [TestMethod]
//        public async Task TestMultipleChanges()
//        {
//            //Arrange
//            var dataSourceMock = new Mock<IDataSource<IEnumerable<CurrencyChangeApiData>>>();
//            var apiChange = new CurrencyChangeApiData { Currency = "BTC", CurrentBid = 100, PreviousDayBid = 50 };
//            var apiData = new List<CurrencyChangeApiData>
//            {
//                apiChange,
//                new CurrencyChangeApiData {Currency = "OneChange", CurrentBid = 100, PreviousDayBid = 9}
//            };
//            dataSourceMock.Setup(m => m.GetData()).ReturnsAsync(apiData);

//            using (var repo = _repoFactory.Create<CurrencyState>())
//            {
//                await repo.Add(new CurrencyState
//                {
//                    Currency = "BTC",
//                    LastChangeTime = DateTime.Now - TimeSpan.FromMinutes(20),
//                    Threshold = 10,
//                    Created = DateTime.MinValue
//                });

//                await repo.Add(new CurrencyState
//                {
//                    Currency = "NotChanged",
//                    LastChangeTime = DateTime.Now,
//                    Threshold = 10
//                });

//                await repo.Add(new CurrencyState
//                {
//                    Currency = "Expired",
//                    LastChangeTime = DateTime.MinValue,
//                    Threshold = 10
//                });
//            }
//            var settings = new ChangeSettings()
//            {
//                Percentage = 45,
//                MultipleChangesSpanMinutes = 30,
//                MultipleChanges = true
//            };

//            var monitor =
//                new ChangeMonitor(dataSourceMock.Object, _repoFactory, new TestSettingsProvider(settings));

//            var changes = (await monitor.CheckChanges()).ToList();

//            var change = changes.FirstOrDefault();

//            List<CurrencyState> states;
//            List<ChangeHistoryEntry> history;

//            using (var stateRepo = _repoFactory.Create<CurrencyState>())
//            {
//                states = new List<CurrencyState>(stateRepo.GetAll());
//            }

//            using (var historyRepo = _repoFactory.Create<ChangeHistoryEntry>())
//            {
//                history = new List<ChangeHistoryEntry>(historyRepo.GetAll());
//            }

//            var state = states.FirstOrDefault();
//            var historyEntry = history.FirstOrDefault();

//            //Assert
//            Assert.AreEqual(1, changes.Count);
//            Assert.AreEqual(4, states.Count);
//            Assert.AreEqual(1, history.Count);

//            Assert.IsNotNull(change);
//            Assert.AreEqual(apiChange.Currency, change.Currency);
//            Assert.AreEqual(100, change.Percentage);
//            Assert.AreEqual(90, change.Threshold);
//            Assert.AreEqual(UpdateType.Currency, change.Type);
//            Assert.IsTrue(change.Time > DateTime.Now - TimeSpan.FromMinutes(10));

//            Assert.IsNotNull(state);
//            Assert.AreEqual(change.Currency, state.Currency);
//            Assert.AreEqual(90, state.Threshold);
//            Assert.IsTrue(state.LastChangeTime > DateTime.Now - TimeSpan.FromMinutes(10));
//            Assert.AreEqual(DateTime.MinValue, state.Created);

//            Assert.IsNotNull(historyEntry);
//            Assert.AreEqual(change.Currency, historyEntry.Currency);
//            Assert.AreEqual(100, historyEntry.Percentage);
//            Assert.AreEqual(UpdateType.Currency, historyEntry.Type);
//            Assert.IsTrue(historyEntry.Time > DateTime.Now - TimeSpan.FromMinutes(10));
//        }

//        [TestMethod]
//        public async Task TestMultipleQuickChanges()
//        {
//            //Arrange
//            var dataSourceMock = new Mock<IDataSource<IEnumerable<CurrencyChangeApiData>>>();
//            var apiChange = new CurrencyChangeApiData { Currency = "MUSIC", CurrentBid = 105, PreviousDayBid = 100 };
//            var apiChange1 = new CurrencyChangeApiData { Currency = "MUSIC", CurrentBid = 108, PreviousDayBid = 100 };
//            var apiChange2 = new CurrencyChangeApiData { Currency = "MUSIC", CurrentBid = 112, PreviousDayBid = 100 };
//            var apiData = new List<CurrencyChangeApiData>
//            {
//                apiChange,
//            };
//            dataSourceMock.Setup(m => m.GetData()).ReturnsAsync(apiData);



//            var settings = new ChangeSettings()
//            {
//                Percentage = 5,
//                MultipleChangesSpanMinutes = 2,
//                MultipleChanges = true
//            };

//            var monitor =
//                new ChangeMonitor(dataSourceMock.Object, _repoFactory, new TestSettingsProvider(settings));

//            await monitor.CheckChanges();

//            apiData[0] = apiChange1;
//            await monitor.CheckChanges();

//            apiData[0] = apiChange2;
//            await monitor.CheckChanges();

//            List<ChangeHistoryEntry> history;

//            using (var historyRepo = _repoFactory.Create<ChangeHistoryEntry>())
//            {
//                history = new List<ChangeHistoryEntry>(historyRepo.GetAll());
//            }

//            Assert.AreEqual(1, history.Count);
//        }

//        [TestMethod]
//        public async Task TestPriceDecreaseDetectedInMarginCurrencies()
//        {
//            //Arrange
//            var dataSourceMock = new Mock<IDataSource<IEnumerable<CurrencyChangeApiData>>>();
//            var marginDecreased = new CurrencyChangeApiData { Currency = "MD", CurrentBid = 10, PreviousDayBid = 100 };
//            var regularDecreased = new CurrencyChangeApiData { Currency = "RD", CurrentBid = 10, PreviousDayBid = 100 };
//            var marginIncrease = new CurrencyChangeApiData { Currency = "MI", CurrentBid = 200, PreviousDayBid = 100 };
//            var regularIncrease = new CurrencyChangeApiData { Currency = "RI", CurrentBid = 200, PreviousDayBid = 100 };
//            var marginNotChanged = new CurrencyChangeApiData { Currency = "MN", CurrentBid = 105, PreviousDayBid = 100 };
//            var regularNotChanged = new CurrencyChangeApiData { Currency = "RN", CurrentBid = 105, PreviousDayBid = 100 };

//            var apiData = new List<CurrencyChangeApiData>
//            {
//                marginDecreased, regularDecreased, marginIncrease, regularIncrease, marginNotChanged, regularNotChanged
//            };
//            dataSourceMock.Setup(m => m.GetData()).ReturnsAsync(apiData);



//            var settings = new ChangeSettings()
//            {
//                Percentage = 30,
//                MultipleChanges = false,
//                MarginCurrencies = new List<string>(new[] { "MD", "MI", "MN" }),
//                MarginPercentage = 30
//            };

//            var monitor =
//                new ChangeMonitor(dataSourceMock.Object, _repoFactory, new TestSettingsProvider(settings));

//            await monitor.CheckChanges();


//            List<ChangeHistoryEntry> history;

//            using (var historyRepo = _repoFactory.Create<ChangeHistoryEntry>())
//            {
//                history = new List<ChangeHistoryEntry>(historyRepo.GetAll());
//            }

//            Assert.AreEqual(3, history.Count);
//            Assert.IsTrue(history.Exists(h => h.Currency == "MD"));
//            Assert.IsTrue(history.Exists(h => h.Currency == "MI"));
//            Assert.IsTrue(history.Exists(h => h.Currency == "RI"));
//        }

//        [TestMethod]
//        public async Task TestDontSignalTwoMarginChangesInDifferentDirections()
//        {
//            //Arrange
//            var dataSourceMock = new Mock<IDataSource<IEnumerable<CurrencyChangeApiData>>>();
//            var apiDecrease1 = new CurrencyChangeApiData { Currency = "BTC", CurrentBid = 10, PreviousDayBid = 100 };
//            var apiIncrease1 = new CurrencyChangeApiData { Currency = "BTC", CurrentBid = 200, PreviousDayBid = 100 };

//            var apiIncrease2 = new CurrencyChangeApiData { Currency = "BTC1", CurrentBid = 200, PreviousDayBid = 100 };
//            var apiDecrease2 = new CurrencyChangeApiData { Currency = "BTC1", CurrentBid = 10, PreviousDayBid = 100 };

//            var apiData = new List<CurrencyChangeApiData>
//            {
//                apiDecrease1, apiIncrease2
//            };
//            dataSourceMock.Setup(m => m.GetData()).ReturnsAsync(apiData);

//            var settings = new ChangeSettings()
//            {
//                Percentage = 5,
//                MultipleChangesSpanMinutes = 20,
//                MultipleChanges = true,
//                MarginPercentage = 5,
//                MarginCurrencies = new List<string> { "BTC", "BTC1" }
//            };

//            var monitor =
//                new ChangeMonitor(dataSourceMock.Object, _repoFactory, new TestSettingsProvider(settings));

//            //Act
//            await monitor.CheckChanges();
//            apiData[0] = apiIncrease1;
//            apiData[1] = apiDecrease2;
//            await monitor.CheckChanges();
          

//            List<ChangeHistoryEntry> history;
//            using (var historyRepo = _repoFactory.Create<ChangeHistoryEntry>())
//            {
//                history = new List<ChangeHistoryEntry>(historyRepo.GetAll());
//            }

//            Assert.AreEqual(0, history.Count);
//        }

//        [TestMethod]
//        public async Task TestMultipleMarginChangesInSameDirection()
//        {
//            //Arrange
//            var dataSourceMock = new Mock<IDataSource<IEnumerable<CurrencyChangeApiData>>>();
//            var apiDecrease1 = new CurrencyChangeApiData { Currency = "BTC", CurrentBid = 50, PreviousDayBid = 100 };
//            var apiDecrease2 = new CurrencyChangeApiData { Currency = "BTC", CurrentBid = 10, PreviousDayBid = 100 };

//            var apiIncrease1 = new CurrencyChangeApiData { Currency = "BTC1", CurrentBid = 200, PreviousDayBid = 100 };
//            var apiIncrease2 = new CurrencyChangeApiData { Currency = "BTC1", CurrentBid = 300, PreviousDayBid = 100 };

//            var apiData = new List<CurrencyChangeApiData>
//            {
//                apiDecrease1, apiIncrease1
//            };
//            dataSourceMock.Setup(m => m.GetData()).ReturnsAsync(apiData);

//            var settings = new ChangeSettings()
//            {
//                Percentage = 5,
//                MultipleChangesSpanMinutes = 20,
//                MultipleChanges = true,
//                MarginPercentage = 5,
//                MarginCurrencies = new List<string> { "BTC", "BTC1" }
//            };

//            var monitor =
//                new ChangeMonitor(dataSourceMock.Object, _repoFactory, new TestSettingsProvider(settings));

//            //Act
//            await monitor.CheckChanges();
//            apiData[0] = apiDecrease2;
//            apiData[1] = apiIncrease2;
//            await monitor.CheckChanges();


//            List<ChangeHistoryEntry> history;
//            using (var historyRepo = _repoFactory.Create<ChangeHistoryEntry>())
//            {
//                history = new List<ChangeHistoryEntry>(historyRepo.GetAll());
//            }

//            Assert.AreEqual(2, history.Count);
//            Assert.IsTrue(history.Exists(h=>h.Currency == "BTC"));
//            Assert.IsTrue(history.Exists(h=>h.Currency == "BTC1"));
//        }

//        [TestMethod]
//        public async Task TestDiscardMarginDecreaseBelowThreshold()
//        {
//            //Arrange
//            var dataSourceMock = new Mock<IDataSource<IEnumerable<CurrencyChangeApiData>>>();
//            var apiValue1 = new CurrencyChangeApiData { Currency = "BTC", CurrentBid = 200, PreviousDayBid = 100 };
//            var apiValue2 = new CurrencyChangeApiData { Currency = "BTC", CurrentBid = 180, PreviousDayBid = 100 };
            
//            var apiData = new List<CurrencyChangeApiData>
//            {
//                apiValue1
//            };
//            dataSourceMock.Setup(m => m.GetData()).ReturnsAsync(apiData);

//            var settings = new ChangeSettings()
//            {
//                Percentage = 5,
//                MultipleChangesSpanMinutes = 20,
//                MultipleChanges = true,
//                MarginPercentage = 0.5,
//                MarginCurrencies = new List<string> { "BTC" }
//            };

//            var monitor =
//                new ChangeMonitor(dataSourceMock.Object, _repoFactory, new TestSettingsProvider(settings));

//            //Act
//            await monitor.CheckChanges();
//            apiData[0] = apiValue2;
//            await monitor.CheckChanges();


//            List<ChangeHistoryEntry> history;
//            using (var historyRepo = _repoFactory.Create<ChangeHistoryEntry>())
//            {
//                history = new List<ChangeHistoryEntry>(historyRepo.GetAll());
//            }

//            Assert.AreEqual(0, history.Count);
            
//        }
//    }
//}