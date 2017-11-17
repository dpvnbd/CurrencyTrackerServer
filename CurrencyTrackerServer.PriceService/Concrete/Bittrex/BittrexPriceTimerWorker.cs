using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Abstract.Price;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Price;
using Microsoft.Extensions.Options;

namespace CurrencyTrackerServer.PriceService.Concrete.Bittrex
{
    public class BittrexPriceTimerWorker:PriceTimerWorker
    {
        public override UpdateSource Source => UpdateSource.Bittrex;

        public BittrexPriceTimerWorker(BittrexPriceDataSource dataSource, INotifier notifier,
            ISettingsProvider settingsProvider, IOptions<AppSettings> config) : base(dataSource, notifier, settingsProvider, config)
        {
        }
    }
}
