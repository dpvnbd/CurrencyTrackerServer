using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Abstract.Price;
using CurrencyTrackerServer.Infrastructure.Abstract.Workers;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;
using CurrencyTrackerServer.Infrastructure.Entities.Price;
using Microsoft.Extensions.Options;

namespace CurrencyTrackerServer.PriceService.Concrete
{
    public abstract class PriceTimerWorker : AbstractTimerWorker<IEnumerable<ApiPrice>>
    {
        private readonly IPriceSource _dataSource;
        private readonly INotifier _notifier;

        public PriceTimerWorker(IPriceSource dataSource, INotifier notifier,
            ISettingsProvider settingsProvider, IOptions<AppSettings> config)
        {
            _dataSource = dataSource;
            _notifier = notifier;
            Period = config.Value.PriceWorkerPeriodSeconds * 1000;
        }

        protected override async Task DoWork()
        {
            try
            {
                var prices = await _dataSource.GetPrices();
                if (prices.Any())
                {
                    OnUpdated(prices);
                }
            }
            catch (Exception e)
            {
                var errorMessage = new Price
                {
                    Message = e.Message,
                    //Source = Monitor.Source,
                    Type = UpdateType.Error
                };

                await _notifier.SendToAll(new[] { errorMessage });
            }
        }

        public async Task<Price> GetPrice(string currency)
        {
            try
            {
                var prices = await _dataSource.GetPrices();
                var price = prices.FirstOrDefault(p => string.Equals(p.Currency, currency, StringComparison.OrdinalIgnoreCase));
                if (price == null) throw new Exception();
                return new Price { Currency = currency, Last = price.Last, Source = price.Source };
            }
            catch (Exception)
            {
                return new Price
                {
                    Message = "Ошибка получения валюты " + currency,
                    Source = Source,
                    Destination = UpdateDestination.Price,
                    Type = UpdateType.Error
                };
            }
        }
    }
}
