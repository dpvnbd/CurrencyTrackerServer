using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Price;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;
using CurrencyTrackerServer.Infrastructure.Entities.Price;

namespace CurrencyTrackerServer.PriceService.Concrete
{
    public class PriceTimerWorker:AbstractTimerWorker
    {
        public IPriceMonitor<Price> Monitor { get; }
        private readonly INotifier<Price> _notifier;

        public PriceTimerWorker(INotifier<Price> notifier, IPriceMonitor<Price> monitor)
        {
            _notifier = notifier;
            Monitor = monitor;
        }

        protected override async Task DoWork()
        {
            try
            {
                var prices = await Monitor.GetPrices();
                if (prices.Any())
                {
                    await _notifier.SendNotificationMessage(prices);
                }

            }
            catch (Exception e)
            {
                var errorMessage = new Price
                {
                    Message = e.Message,
                    Source = Monitor.Source
                };

                await _notifier.SendNotificationMessage(errorMessage);
            }
            finally
            {
                Period = Monitor.Settings.PeriodSeconds * 1000;
            }
        }
    }
}
