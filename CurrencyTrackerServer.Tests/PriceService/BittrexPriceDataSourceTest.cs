using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.PriceService.Concrete.Bittrex;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CurrencyTrackerServer.Tests.PriceService
{
    [TestClass]
    public class BittrexPriceDataSourceTest
    {
        [TestMethod]
        public void JsonParsePriceTest()
        {
        var json = @"{
	        ""success"" : true,
	        ""message"" : """",
	        ""result"" : {
		        ""Bid"" : 2.05670368,
		        ""Ask"" : 3.35579531,
		        ""Last"" : 3.35579531
	        }
        }";
            var dataSource = new BittrexPriceDataSource();

            var price = dataSource.ParseResponse(json);

            Assert.IsNotNull(price);
            Assert.IsTrue(Math.Abs(3.35579531 - price.Last) < 0.00000001);
        }

        [TestMethod]
        public void NotSuccessfulTest()
        {
            var json = @"{
	        ""success"" : false,
	        ""message"" : """",
	        ""result"" : {
		        ""Bid"" : 2.05670368,
		        ""Ask"" : 3.35579531,
		        ""Last"" : 3.35579531
	        }
        }";
            var dataSource = new BittrexPriceDataSource();

            var price = dataSource.ParseResponse(json);

            Assert.IsNull(price);
        }

        [TestMethod]
        public void WrongFormatTest()
        {
            var json = @"{
	        ""success"" : fe,
	        ""message"" : """",
	        ""result"" : {
		        ""Bid"" : 2.05670368,
		        ""Ask"" : 3.35579531,
		        ""Last"" : 3.35579531
	        }
        }";
            var dataSource = new BittrexPriceDataSource();

            var price = dataSource.ParseResponse(json);

            Assert.IsNull(price);
        }

        [TestMethod]
        public void WrongResultFormatTest()
        {
            var json = @"{
	        ""success"" : true,
	        ""message"" : """",
	        ""result1"" : {
		        ""Bid"" : 2.05670368,
		        ""Ask"" : 3.35579531,
		        ""Last"" : 3.35579531
	        }
        }";
            var dataSource = new BittrexPriceDataSource();

            var price = dataSource.ParseResponse(json);

            Assert.IsNull(price);
        }
    }
}