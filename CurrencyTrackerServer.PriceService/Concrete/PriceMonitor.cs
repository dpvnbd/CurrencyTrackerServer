using System;
using System.Collections.Generic;
using System.Linq;
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

            var currencies = Settings.Prices.Select(p => p.Currency);


            try
            {
                var apiPrices = await _dataSource.GetPrices(currencies);
                foreach (var apiPrice in apiPrices)
                {
                    var price = Settings.Prices.SingleOrDefault(c => c.Currency == apiPrice.Currency);
                    if (price == null)
                    {
                        continue;
                    }
                    price.Last = apiPrice.Last;
                    price.Source = apiPrice.Source;
                    prices.Add(price);
                }
            }
            catch (Exception)
            {
                //Console.WriteLine("Price monitor error");
            }
            return prices;
        }

        public async Task<Price> GetPrice(string currency)
        {
            try
            {
                var price = await _dataSource.GetPrices(new []{ currency });
                if (price.First() == null) return null;
                return new Price {Currency = currency, Last = price.First().Last, Source = price.First().Source };
            }
            catch (Exception)
            {
                return new Price {Message = "Ошибка получения валюты " + currency};
            }
        }
    }
}