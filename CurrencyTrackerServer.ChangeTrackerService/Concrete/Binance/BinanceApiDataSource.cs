using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Entities;
using Newtonsoft.Json;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.Binance
{
  public class BinanceApiDataSource : IDataSource<IEnumerable<CurrencyChangeApiData>>
  {
    public UpdateSource Source => UpdateSource.Binance;

    public async Task<IEnumerable<CurrencyChangeApiData>> GetData()
    {
      var list = new List<CurrencyChangeApiData>();
      try
      {
        string json;
        using (var client = new HttpClient())
        {
          json = await client.GetStringAsync("https://api.binance.com/api/v1/ticker/24hr");
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

          string bid = c.bidPrice;

          var current = double.Parse(bid, CultureInfo.InvariantCulture);

          string percentageString = c.priceChangePercent.ToString();
          var percent = Double.Parse(percentageString, CultureInfo.InvariantCulture);

          var price = new CurrencyChangeApiData()
          {
            Currency = currency,
            CurrentBid = current,
            PercentChanged = percent,
            UpdateSource = UpdateSource.Binance
          };
          list.Add(price);
        }
      }
      catch (Exception e)
      {
        throw new Exception("Ошибка загрузки значений", e);
      }

      return list;
    }

  }
}
