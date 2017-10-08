using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Price;
using CurrencyTrackerServer.Infrastructure.Entities.Price;

namespace CurrencyTrackerServer.PriceService.Concrete.Poloniex
{
    public class PoloniexPriceTimerWorker:PriceTimerWorker
    {
        public PoloniexPriceTimerWorker(INotifier<Price> notifier, PoloniexPriceMonitor monitor) : base(notifier, monitor)
        {
        }
    }
}
