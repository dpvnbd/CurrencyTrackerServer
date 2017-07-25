using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CurrencyTrackerServer.Data;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Services.Abstract;
using Newtonsoft.Json;

namespace CurrencyTrackerServer.Services.Concrete
{
    public class BittrexService : IBittrexService
    {


        public IEnumerable<BittrexChange> LoadChanges()
        {
            string json;
            var list = new List<BittrexChange>();
            try
            {
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
                    string prevDay = c.PrevDay;
                    string bid = c.Bid;

                    double current = double.Parse(bid, CultureInfo.InvariantCulture);
                    double previous = double.Parse(prevDay, CultureInfo.InvariantCulture);
                    if (Math.Abs(previous) < 0.00000001)
                        continue;
                    var currencyChange = new BittrexChange()
                    {
                        ReferenceCurrency = markets[0],
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

        public IEnumerable<BittrexChange> LoadChangesGreaterThan(int percentage)
        {
            return LoadChangesGreaterThan(percentage, false, TimeSpan.Zero);
        }

        public IEnumerable<BittrexChange> LoadChangesGreaterThan(int percentage,
            bool changeOverTime, TimeSpan interval)
        {
            
            var list = LoadChanges();
            var changed = new List<BittrexChange>(list.Where(c => c.ChangePercentage >= percentage));
            var now = DateTime.Now;
            using (var repo = new BittrexHistoryRepository(new BittrexContext()))
            {
                repo.ResetHistoryIfOlderThan(AutoResetHours);
                foreach (var c in changed.ToList())
                {
                    var entity = repo.Find(c.Currency);
                    var changeTime = DateTime.MinValue;
                    int lastThreshold = 0;

                    if (entity != null)
                    {
                        changeTime = entity.ChangeTime;
                        lastThreshold = entity.Threshsold;
                    }

                    var item = changed.FirstOrDefault(x => x.Currency == c.Currency);

                    if ((int) c.ChangePercentage < lastThreshold + percentage)
                    {
                        changed.Remove(item);
                        
                        continue;
                    }

                    c.ChangeTime = now;

                    c.Threshsold = (int) (c.ChangePercentage / percentage) * percentage;


                    if (entity == null)
                    {
                        c.CreatedTime = DateTime.Now;
                        repo.Add(c);
                    }
                    else
                    {
                        entity.Threshsold = c.Threshsold;
                        entity.ChangeTime = c.ChangeTime;
                        entity.CurrentBid = c.CurrentBid;
                        entity.PreviousDayBid = c.PreviousDayBid;
                    }
                    repo.Save();

                    if (changeOverTime && (now - changeTime) > interval)
                    {
                        changed.Remove(item);
                        continue;
                    }

                    entity = repo.Find(c.Currency);
                    if (entity != null)
                    {
                        entity.LastNotifiedChange = DateTime.Now;
                        repo.Save();
                        c.LastNotifiedChange = entity.LastNotifiedChange;

                    }
                }
            }

            return changed;
        }

        public double AutoResetHours { get; set; } = 24;
    }
}