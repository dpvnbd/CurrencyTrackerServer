using System;
using System.Collections.Generic;
using System.Text;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Abstract.Price;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Price;

namespace CurrencyTrackerServer.PriceService.Concrete.Bittrex
{
    public class BittrexPriceMonitor : PriceMonitor
    {
        public BittrexPriceMonitor(BittrexPriceTimerWorker timerWorker, ISettingsProvider settingsProvider,
            IMessageNotifier notifier, string userId) 
            : base(timerWorker, settingsProvider, notifier, userId)
        {
        }

        public override UpdateSource Source => UpdateSource.Bittrex;
    }
}