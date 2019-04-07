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

namespace CurrencyTrackerServer.PriceService.Concrete.Binance
{
  public class BinancePriceDataSource : IPriceSource
  {
    public async Task<IEnumerable<ApiPrice>> GetPrices()
    {
      var list = new List<ApiPrice>();
      try
      {
        string json;
        using (var client = new HttpClient())
        {
          json = await client.GetStringAsync("https://api.binance.com/api/v3/ticker/price");
        }
        dynamic result = JsonConvert.DeserializeObject(json);
      
        foreach (dynamic c in result)
        {
          string symbol = c.symbol.ToString();
          string baseCurrency = symbol.Substring(symbol.Length - 3);
          if (baseCurrency != "BTC")
          {
            continue;
          }
          string currency = symbol.Remove(symbol.Length - 3);
          string responsePrice = c.price;
          var price = double.Parse(responsePrice, CultureInfo.InvariantCulture);
          var currencyChange = new ApiPrice()
          {
            Currency = currency,
            Last = price,
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

    public UpdateSource Source => UpdateSource.Binance;
  }
}