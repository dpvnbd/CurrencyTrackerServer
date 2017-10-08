using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Price;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Price;

namespace CurrencyTrackerServer.PriceService.Concrete.Poloniex
{
    public class PoloniexPriceMonitor : PriceMonitor
    {
        public PoloniexPriceMonitor(PoloniexPriceDataSource dataSource,
            ISettingsProvider<PriceSettings> settingsProvider) : base(dataSource, settingsProvider)
        {
            Source = ChangeSource.Poloniex;
        }
    }
}