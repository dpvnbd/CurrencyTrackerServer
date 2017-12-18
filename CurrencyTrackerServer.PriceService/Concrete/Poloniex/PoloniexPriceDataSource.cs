using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.Infrastructure.Abstract.Price;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Price;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CurrencyTrackerServer.PriceService.Concrete.Poloniex
{
    public class PoloniexPriceDataSource : IPriceSource
    {
        public UpdateSource Source => UpdateSource.Poloniex;

        private readonly string _url = @"https://poloniex.com/public?command=returnTicker";

    public IEnumerable<ApiPrice> GetPrices()
    {
      var prices = new List<ApiPrice>();

      try
      {
        string json;
        using (var client = new HttpClient())
        {
          var result = client.GetAsync(_url).Result;
          if (!result.IsSuccessStatusCode)
          {
            throw new Exception("Ошибка работы с API (Poloniex)");
          }
          json = result.Content.ReadAsStringAsync().Result;
        }
        var ticker = ParseResponse(json);

        return ticker;
      }
      catch (Exception e)
      {
        throw new Exception("Ошибка загрузки значений (Poloniex); ", e);
      }
    }

    public List<ApiPrice> ParseResponse(string json)
        {
            var list = new List<ApiPrice>();

            var converter = new ExpandoObjectConverter();
            dynamic pairs = JsonConvert.DeserializeObject<ExpandoObject>(json, converter);
            foreach (var pair in (IDictionary<String, Object>)pairs)
            {
                string currencyPair = pair.Key;
                string[] currencies = currencyPair.Split('_');
                if (currencies[0] != "BTC")
                {
                    continue; //Only bitcoin pairs
                }
                dynamic value = pair.Value;
                string priceString = value.last.ToString();
                var price = double.Parse(priceString, CultureInfo.InvariantCulture);

                list.Add(new ApiPrice()
                {
                    Currency = currencies[1],
                    Source = Source,
                    Last = price
                });
            }
            return list;
        }
    }
}