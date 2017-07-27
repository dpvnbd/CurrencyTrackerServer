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
    public class CalculatePercentageTests
    {
        [TestMethod]
        public void TestChangeCalculation()
        {
            var hundred = new BittrexApiData
            {
                CurrentBid = 12,
                PreviousDayBid = 6
            };
            var infinity = new BittrexApiData
            {
                CurrentBid = 12,
                PreviousDayBid = 0
            };

            var zero = new BittrexApiData
            {
                CurrentBid = 12,
                PreviousDayBid = 12
            };

            var negative = new BittrexApiData
            {
                CurrentBid = 12,
                PreviousDayBid = 15
            };

            var minusHundred = new BittrexApiData
            {
                CurrentBid = 0,
                PreviousDayBid = 15
            };

            Assert.AreEqual(100, hundred.PercentChanged);
            Assert.AreEqual(0, zero.PercentChanged);
            Assert.IsTrue(negative.PercentChanged < 0);
            Assert.IsTrue(double.IsInfinity(infinity.PercentChanged));
            Assert.AreEqual(-100, minusHundred.PercentChanged);

        }

        [TestMethod]
        public void TestThresholdCalculation()
        {
            var exact = CurrencyStateEntity.CalculateThreshold(5, 15);
            var over = CurrencyStateEntity.CalculateThreshold(5, 16);
            var under = CurrencyStateEntity.CalculateThreshold(5, 14);
            var zero = CurrencyStateEntity.CalculateThreshold(0, 14);

            Assert.AreEqual(15, exact);
            Assert.AreEqual(15, over);
            Assert.AreEqual(10, under);
            Assert.AreEqual(0, zero);
        }
    }
}