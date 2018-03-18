using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.Poloniex
{
    public class PoloniexApiDataSource : IDataSource<IEnumerable<CurrencyChangeApiData>>
    {
        const string _url = @"https://poloniex.com/public?command=returnTicker";
        public UpdateSource Source => UpdateSource.Poloniex;

        public List<CurrencyChangeApiData> ParseResponse(string json)
        {
            var list = new List<CurrencyChangeApiData>();

            var converter = new ExpandoObjectConverter();
            dynamic pairs = JsonConvert.DeserializeObject<ExpandoObject>(json, converter);
            foreach (var pair in (IDictionary<String, Object>) pairs)
            {
                string currencyPair = pair.Key;
                string[] currencies = currencyPair.Split('_');
                if (currencies[0] != "BTC")
                {
                    continue; //пропускаем все что не биткоины
                }
                dynamic value = pair.Value;
                string percentageString = value.percentChange.ToString();
                var percent = Double.Parse(percentageString, CultureInfo.InvariantCulture);

                list.Add(new CurrencyChangeApiData()
                {
                    Currency = currencies[1],
                    PercentChanged = percent * 100,
                    UpdateSource = UpdateSource.Poloniex
                });
            }
            return list;
        }

        public async Task<IEnumerable<CurrencyChangeApiData>> GetData()
        {
            var list = new List<CurrencyChangeApiData>();
            try
            {
                string json;
                using (var client = new HttpClient())
                {
                    var result = await client.GetAsync(_url);
                    if (!result.IsSuccessStatusCode)
                    {
                        throw new Exception("Ошибка работы с API (Poloniex)");
                    }
                    json = await result.Content.ReadAsStringAsync();
                }
                list = ParseResponse(json);
            }
            catch (Exception e)
            {
                throw new Exception("Ошибка загрузки значений (Poloniex); ", e);
            }

            return list;
        }

        public async Task<IEnumerable<string>> GetCurrencies()
        {
            List<string> list;
            try
            {
                string json;
                using (var client = new HttpClient())
                {
                    var result = await client.GetAsync(_url);
                    if (!result.IsSuccessStatusCode)
                    {
                        throw new Exception("Ошибка работы с API (Poloniex)");
                    }
                    json = await result.Content.ReadAsStringAsync();
                }
                var statesList = ParseResponse(json);

                list = statesList.Select(s => s.Currency).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error getting poloniex currencies " + e.Message);
                list = new List<string>();
            }
            return list;

        }

    }
}