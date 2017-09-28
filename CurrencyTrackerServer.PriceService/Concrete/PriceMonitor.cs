using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Price;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Price;

namespace CurrencyTrackerServer.PriceService.Concrete
{
    public class PriceMonitor : IPriceMonitor<Price>
    {
        private readonly IPriceSource _dataSource;
        public ChangeSource Source { get; protected set; }
        public PriceSettings Settings{ get; }

        public PriceMonitor(IPriceSource dataSource, ISettingsProvider<PriceSettings> settingsProvider)
        {
            _dataSource = dataSource;
            Source = dataSource.Source;
            Settings = settingsProvider.GetSettings(Source);
        }


        public async Task<IEnumerable<Price>> GetPrices()
        {
            var prices = new List<Price>();
            foreach (var currency in Settings.Currencies)
            {
                ApiPrice price;

                try
                {
                    price = await _dataSource.GetPrice(currency.Currency);
                }
                catch (Exception)
                {
                    price = null;
                }
                if (price == null)
                {
                    currency.Last = -1;
                }
                else
                {
                    currency.Last = price.Last;
                }

                prices.Add(currency);
            }

            return prices;
        }
    }
}