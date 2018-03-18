using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Abstract.Price;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Price;
using Microsoft.Extensions.Options;

namespace CurrencyTrackerServer.PriceService.Concrete.Poloniex
{
    public class PoloniexPriceTimerWorker : PriceTimerWorker
    {
        public PoloniexPriceTimerWorker(PoloniexPriceDataSource source,
            INotifier notifier, ISettingsProvider settings, IOptions<AppSettings> config) : base(source, notifier, settings, config)
        {
        }

        public override UpdateSource Source => UpdateSource.Poloniex;
    }
}
