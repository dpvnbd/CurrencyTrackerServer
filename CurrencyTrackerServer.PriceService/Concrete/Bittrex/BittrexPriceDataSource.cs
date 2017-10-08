using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Price;
using CurrencyTrackerServer.Infrastructure.Entities;
using Newtonsoft.Json;

namespace CurrencyTrackerServer.PriceService.Concrete.Bittrex
{
    public class BittrexPriceDataSource : IPriceSource
    {
        private string _url = @"https://bittrex.com/api/v1.1/public/getticker?market=BTC-";

        public async Task<IEnumerable<ApiPrice>> GetPrices(IEnumerable<string> currencies)
        {
            var prices = new List<ApiPrice>();

            foreach (var currency in currencies)
            {
                string json;
                try
                {
                    using (var client = new HttpClient())
                    {
                        var result = await client.GetAsync(_url + currency);
                        if (!result.IsSuccessStatusCode)
                        {
                            throw new Exception("Ошибка работы с API (Poloniex)");
                        }
                        json = await result.Content.ReadAsStringAsync();
                    }
                    var price = ParseResponse(json);

                    if (price != null)
                    {
                        price.Currency = currency.ToUpperInvariant();
                        price.Source = ChangeSource.Bittrex;
                        prices.Add(price);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Ошибка загрузки значений (Bittrex); ", e);
                }
            }
            return prices;
        }

        public ApiPrice ParseResponse(string json)
        {
            ApiPrice price = null;

            try
            {
                dynamic response = JsonConvert.DeserializeObject(json);
                bool success = response.success;

                if (success)
                {
                    price = new ApiPrice
                    {
                        Last = response.result.Last
                    };
                }
                else
                {
                    Console.WriteLine(response.message);
                }
            }
            catch (Exception)
            {
                price = null;
            }
            return price;
        }

        public ChangeSource Source => ChangeSource.Bittrex;
    }
}