using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CurrencyTrackerServer.ChangeTrackerService.Concrete;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Poloniex;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CurrencyTrackerServer.Tests.ChangeTrackerService
{
    [TestClass]
    public class PoloniexApiTests
    {
        [TestMethod]
        public void TestApiResponseParsing()
        {
            var dataSource = new PoloniexApiDataSource();
            var json =
                "{\"BTC_BCN\":{\"id\":7,\"last\":\"0.00000043\",\"lowestAsk\":\"0.00000044\",\"highestBid\":\"0.00000043\"," +
                "\"percentChange\":\"-0.02272727\",\"baseVolume\":\"118.28301195\",\"quoteVolume\":\"270280291.22187871\",\"isFrozen\":\"0\",\"high24hr\":\"0.00000045\",\"low24hr\":\"0.00000042\"}," +
                "\"BTC_BELA\":{\"id\":8,\"last\":\"0.00003651\",\"lowestAsk\":\"0.00003683\",\"highestBid\":\"0.00003652\"," +
                "\"percentChange\":\"0.03252262\",\"baseVolume\":\"13.25175178\",\"quoteVolume\":\"360027.09759833\",\"isFrozen\":\"0\",\"high24hr\":\"0.00003822\",\"low24hr\":\"0.00003525\"}}";

            var result = dataSource.ParseResponse(json).ToArray();

            Assert.AreEqual("BCN", result[0].Currency);
            Assert.IsTrue(Math.Abs(-2.2 - result[0].PercentChanged) < 0.1);

            Assert.AreEqual("BELA", result[1].Currency);
            Assert.IsTrue(Math.Abs(3.2 - result[1].PercentChanged) < 0.1);
        }
    }
}