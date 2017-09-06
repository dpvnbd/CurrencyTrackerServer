using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete
{
    public class PoloniexApiDataSource:IDataSource<IEnumerable<CurrencyChangeApiData>>
    {

        public IEnumerable<CurrencyChangeApiData> ParseResponse(string json)
        {
            var list = new List<CurrencyChangeApiData>();

            var converter = new ExpandoObjectConverter();
            dynamic pairs = JsonConvert.DeserializeObject<ExpandoObject>(json, converter);
            foreach (var pair in (IDictionary<String, Object>)pairs)
            {
                string currencyPair = pair.Key;
                string[] currencies = currencyPair.Split('_');
                if (currencies[0] != "BTC")
                {
                    continue; //пропускаем все что не биткоины
                }
                dynamic value = pair.Value;
                string priceString = value.percentChange.ToString();
                var price = Double.Parse(priceString, CultureInfo.InvariantCulture);
                list.Add(new CurrencyChangeApiData(){Currency = currencies[1],PercentChanged = price * 100});
            }
            return list;
        }

        public async Task <IEnumerable<CurrencyChangeApiData>> GetData()
        {
            var list = new List<CurrencyChangeApiData>();
            try
            {
                string json;
                using (var client = new HttpClient())
                {
                    json = await client.GetStringAsync("https://bittrex.com/api/v1.1/public/getmarketsummaries");
                    
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
                    string prevDay = c.PrevDay;
                    string bid = c.Bid;

                    var current = double.Parse(bid, CultureInfo.InvariantCulture);
                    var previous = double.Parse(prevDay, CultureInfo.InvariantCulture);
                    if (Math.Abs(previous) < 0.00000001)
                        continue;
                    var currencyChange = new CurrencyChangeApiData()
                    {
                        Currency = markets[1],
                        PreviousDayBid = previous,
                        CurrentBid = current,
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
    }
}
