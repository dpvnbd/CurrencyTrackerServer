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
        private readonly ISettingsProvider<PriceSettings> _settingsProvider;
        public ChangeSource Source { get; protected set; }
        public PriceSettings Settings { get; private set; }

        public PriceMonitor(IPriceSource dataSource, ISettingsProvider<PriceSettings> settingsProvider)
        {
            _dataSource = dataSource;
            _settingsProvider = settingsProvider;
            Source = dataSource.Source;
            Settings = _settingsProvider.GetSettings(Source);

        }


        public async Task<IEnumerable<Price>> GetPrices()
        {
            Settings = _settingsProvider.GetSettings(Source);

            var prices = new List<Price>();
            foreach (var currency in Settings.Prices)
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
                    currency.Source = price.Source;
                }

                prices.Add(currency);
            }

            return prices;
        }

        public async Task<Price> GetPrice(string currency)
        {
            try
            {
                var price = await _dataSource.GetPrice(currency);
                return new Price {Currency = currency, Last = price.Last, Source = price.Source};
            }
            catch (Exception)
            {
                return new Price {Message = "Ошибка получения валюты " + currency};
            }
        }
    }
}