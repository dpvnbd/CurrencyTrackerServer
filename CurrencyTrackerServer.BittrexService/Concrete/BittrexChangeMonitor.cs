using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyTrackerServer.BittrexService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTrackerServer.BittrexService.Concrete
{
    public class BittrexChangeMonitor<TStateRepo, THistoryRepo> : IChangeMonitor<List<Change>>
        where TStateRepo : IRepository<CurrencyStateEntity>, new()
        where THistoryRepo : IRepository<ChangeHistoryEntryEntity>, new()

    {
        private IDataSource<List<BittrexApiData>> _dataSource;

        public BittrexChangeMonitor(IDataSource<List<BittrexApiData>> dataSource)
        {
            _dataSource = dataSource;
        }


        public async Task<List<Change>> GetChanges(double percentage, TimeSpan multipleChangesSpan,
            bool multipleChanges = false)
        {
            var currencies = await _dataSource.GetData();
            if (currencies.Count == 0)
            {
                return new List<Change>(0);
            }

            var now = DateTime.Now;
            var states = new List<CurrencyStateEntity>();
            var changes = new List<Change>();


            using (var repo = new TStateRepo())
            {
                states = new List<CurrencyStateEntity>(repo.GetAll());
            }

            foreach (var currency in currencies)
            {
                var state = states.FirstOrDefault(s => s.Currency == currency.Currency);
                var threshold = 0d;
                var lastChange = DateTime.MinValue;

                if (state != null)
                {
                    threshold = state.Threshold;
                    lastChange = state.LastChangeTime;
                }

                if (currency.PercentChanged < threshold + percentage)
                {
                    continue;
                }
                var change = new Change
                {
                    Currency = currency.Currency,
                    Percentage = currency.PercentChanged,
                    Time = now,
                    Type = ChangeType.Currency,
                    Threshold = CurrencyStateEntity.CalculateThreshold(percentage, currency.PercentChanged)
                };

                await SaveState(change);

                if (multipleChanges && now - lastChange > multipleChangesSpan)
                {
                    continue;
                }

                changes.Add(change);
            }
            await SaveHistory(changes);
            return changes;
        }

        protected async Task SaveState(Change change)
        {
            using (var repo = new TStateRepo())
            {
                var states = new List<CurrencyStateEntity>();

                states = new List<CurrencyStateEntity>(repo.GetAll());

                var state = states.SingleOrDefault(s => s.Currency == change.Currency);
                if (change.Type != ChangeType.Currency) return;

                if (state == null)
                {
                    state = await repo.Add(new CurrencyStateEntity
                    {
                        Currency = change.Currency,
                        LastChangeTime = change.Time,
                        Threshold = change.Threshold,
                        Created = DateTime.Now
                    });
                }
                else
                {
                    state.LastChangeTime = change.Time;
                    state.Threshold = change.Threshold;
                    await repo.Update(state);
                }
            }
        }

        protected async Task SaveHistory(IEnumerable<Change> changes)
        {
            foreach (var change in changes)
            {
                using (var historyRepo = new THistoryRepo())
                {
                    var entry = new ChangeHistoryEntryEntity
                    {
                        Currency = change.Currency,
                        Message = change.Message,
                        Percentage = change.Percentage,
                        Time = change.Time,
                        Type = change.Type
                    };

                    await historyRepo.Add(entry);
                }
            }
        }

        public List<Change> GetHistory()
        {
            var history = new List<Change>();
            using (var repo = new THistoryRepo())
            {
                var enumerable = repo.GetAll();
                var entities =
                    new List<ChangeHistoryEntryEntity>(enumerable.Skip(Math.Max(0, enumerable.Count() - 50)));
                foreach (var entity in entities)
                {
                    history.Add(new Change
                    {
                        Currency = entity.Currency,
                        Message = entity.Message,
                        Percentage = entity.Percentage,
                        Time = entity.Time,
                        Type = entity.Type
                    });
                }
            }
            return history;
        }

        public async Task ResetAll()
        {
            using (var repo = new TStateRepo())
            {
                await repo.DeleteAll();
            }

            using (var repo = new THistoryRepo())
            {
                await repo.DeleteAll();
                var entry = new ChangeHistoryEntryEntity
                {
                    Type = ChangeType.Info,
                    Message = "Сброс информации о валютах",
                    Time = DateTime.Now
                };

                await repo.Add(entry);
            }
            
            
        }

        public async Task ResetStates(TimeSpan olderThan)
        {
            var now = DateTime.Now;
            using (var repo = new TStateRepo())
            {
                var states = repo.GetAll();
                foreach (var state in states)
                {
                    if (now > state.Created + olderThan)
                    {
                        await repo.Delete(state);
                    }
                }
            }
        }
    }
}