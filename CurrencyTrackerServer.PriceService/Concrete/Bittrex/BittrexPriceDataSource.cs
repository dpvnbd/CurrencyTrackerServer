using System;
using System.Collections.Generic;
using System.Globalization;
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
    public IEnumerable<ApiPrice> GetPrices()
    {
      var list = new List<ApiPrice>();
      try
      {
        string json;
        using (var client = new HttpClient())
        {
          json = client.GetStringAsync("https://bittrex.com/api/v1.1/public/getmarketsummaries").Result;
        }
        dynamic result = JsonConvert.DeserializeObject(json);
        bool success = result.success;
        if (!success)
        {
          throw new Exception("Ошибка работы с API " + result.message ?? "");
        }

        foreach (dynamic c in result.result)
        {
          string[] markets = c.MarketName.ToString().Split('-');
          if (markets[0] != "BTC")
          {
            continue;
          }

          var currencyChange = new ApiPrice()
          {
            Currency = markets[1],
            Last = c.Last,
            Source = Source
          };
          list.Add(currencyChange);
        }
      }
      catch (Exception e)
      {
        throw new Exception("Ошибка загрузки значений", e);
      }

      return list;
    }

    public UpdateSource Source => UpdateSource.Bittrex;
    }
}