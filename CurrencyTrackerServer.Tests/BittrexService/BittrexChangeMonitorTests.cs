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
using CurrencyTrackerServer.Infrastructure.Abstract;

namespace CurrencyTrackerServer.Tests.BittrexService
{
    [TestClass]
    public class BittrexChangeMonitorTests
    {
        private TestRepository<CurrencyStateEntity> _stateRepo;
        private TestRepository<ChangeHistoryEntryEntity> _historyRepo;

        private BittrexChangeMonitor<TestRepository<CurrencyStateEntity>, TestRepository<ChangeHistoryEntryEntity>>
            _monitor;

        [TestInitialize]
        public async Task Setup()
        {
            _monitor = new BittrexChangeMonitor<TestRepository<CurrencyStateEntity>,
                TestRepository<ChangeHistoryEntryEntity>>();
            using (_stateRepo = new TestRepository<CurrencyStateEntity>())
            {
                await _stateRepo.DeleteAll();
            }

            using (_historyRepo = new TestRepository<ChangeHistoryEntryEntity>())
            {
                await _stateRepo.DeleteAll();
            }
        }

        [TestMethod]
        public async Task TestStateReset()
        {
            //Arrange
            using (_stateRepo = new TestRepository<CurrencyStateEntity>())
            {
                await _stateRepo.Add(new CurrencyStateEntity
                {
                    Currency = "BTC",
                    LastChangeTime = DateTime.Now,
                    PercentageThreshold = 45
                });
            }
            
            //Act
            await _monitor.ResetChanges();
            
            //Assert
            using (_stateRepo = new TestRepository<CurrencyStateEntity>())
            {
                var state = _stateRepo.GetAll().FirstOrDefault();
                Assert.IsNull(state);
            }

            using (_historyRepo = new TestRepository<ChangeHistoryEntryEntity>())
            {
                var historyEntry = _historyRepo.GetAll().FirstOrDefault();
                Assert.IsNotNull(historyEntry);
                Assert.IsTrue(historyEntry.Type == HistoryEntryType.Info);
            }
        }
    }
}