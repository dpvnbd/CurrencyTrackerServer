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
    public class PoloniexPriceMonitor : PriceMonitor
    {
        public PoloniexPriceMonitor(PoloniexPriceTimerWorker pWorker,
            ISettingsProvider settingsProvider, IMessageNotifier notifier, string userId)
            : base(pWorker, settingsProvider, notifier, userId)
        {
        }

        public override UpdateSource Source => UpdateSource.Poloniex;
    }
}