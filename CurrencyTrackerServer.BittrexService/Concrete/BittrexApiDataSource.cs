using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.BittrexService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using Newtonsoft.Json;
namespace CurrencyTrackerServer.BittrexService.Concrete
{
    public class BittrexApiDataSource:IDataSource<List<BittrexApiData>>
    {
        public async Task <List<BittrexApiData>> GetData()
        {
            var list = new List<BittrexApiData>();
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
                    var currencyChange = new BittrexApiData()
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
