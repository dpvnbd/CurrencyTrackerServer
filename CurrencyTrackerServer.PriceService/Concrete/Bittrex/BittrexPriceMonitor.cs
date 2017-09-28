using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Price;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Price;

namespace CurrencyTrackerServer.PriceService.Concrete.Bittrex
{
    public class BittrexPriceMonitor : PriceMonitor
    {
        public BittrexPriceMonitor(BittrexPriceDataSource dataSource, ISettingsProvider<PriceSettings> settingsProvider) : base(
            dataSource, settingsProvider)
        {
            Source = ChangeSource.Bittrex;
        }
    }
}