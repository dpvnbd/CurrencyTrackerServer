using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Price;
using CurrencyTrackerServer.Infrastructure.Entities.Price;

namespace CurrencyTrackerServer.PriceService.Concrete.Bittrex
{
    public class BittrexPriceTimerWorker:PriceTimerWorker
    {
        public BittrexPriceTimerWorker(INotifier<Price> notifier, BittrexPriceMonitor monitor) : base(notifier, monitor)
        {
        }
    }
}
