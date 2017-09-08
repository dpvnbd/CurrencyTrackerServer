using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete
{
    public class ChangeMonitor: IChangeMonitor<IEnumerable<Change>>


    {
        private IDataSource<IEnumerable<CurrencyChangeApiData>> _dataSource;
        private readonly IRepository<CurrencyStateEntity> _stateRepo;
        private readonly IRepository<ChangeHistoryEntryEntity> _historyRepo;

        public ChangeMonitor(IDataSource<IEnumerable<CurrencyChangeApiData>> dataSource,
            IRepository<CurrencyStateEntity> stateRepo, IRepository<ChangeHistoryEntryEntity> historyRepo)
        {
            _dataSource = dataSource;
            _stateRepo = stateRepo;
            _historyRepo = historyRepo;
        }


        public async Task<IEnumerable<Change>> GetChanges(double percentage, TimeSpan multipleChangesSpan,
            bool multipleChanges = false)
        {
            var currencies = await _dataSource.GetData();
            if (currencies != null && !currencies.Any())
            {
                return new List<Change>(0);
            }

            var now = DateTime.Now;
            var changes = new List<Change>();
            


            foreach (var currency in currencies)
            {
                var state = _stateRepo.GetAll().FirstOrDefault(s =>
                    s.Currency == currency.Currency && s.ChangeSource == currency.ChangeSource);
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
                    Threshold = CurrencyStateEntity.CalculateThreshold(percentage, currency.PercentChanged),
                    ChangeSource = currency.ChangeSource
                };

                await SaveState(change, state);

                if (multipleChanges && now - lastChange > multipleChangesSpan)
                {
                    continue;
                }

                changes.Add(change);
            }
            await SaveHistory(changes);
            return changes;
        }

        protected async Task SaveState(Change change, CurrencyStateEntity state)
        {
            if (change.Type != ChangeType.Currency) return;
            
            if (state == null)
            {
                await _stateRepo.Add(new CurrencyStateEntity
                {
                    Currency = change.Currency,
                    LastChangeTime = change.Time,
                    Threshold = change.Threshold,
                    Created = DateTime.Now,
                    ChangeSource = change.ChangeSource
                });
            }
            else
            {
                state.LastChangeTime = change.Time;
                state.Threshold = change.Threshold;
                await _stateRepo.Update(state);
            }
        }

        protected async Task SaveHistory(IEnumerable<Change> changes)
        {
            foreach (var change in changes)
            {
                var entry = new ChangeHistoryEntryEntity
                {
                    Currency = change.Currency,
                    Message = change.Message,
                    Percentage = change.Percentage,
                    Time = change.Time,
                    Type = change.Type,
                    ChangeSource = change.ChangeSource
                };

                await _historyRepo.Add(entry);
            }
        }

        public IEnumerable<Change> GetHistory()
        {
            var history = new List<Change>();

            var enumerable = _historyRepo.GetAll();
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
                    Type = entity.Type,
                    ChangeSource = entity.ChangeSource
                });
            }

            return history;
        }

        public async Task ResetAll()
        {
            await _stateRepo.DeleteAll();


            await _historyRepo.DeleteAll();
            var entry = new ChangeHistoryEntryEntity
            {
                Type = ChangeType.Info,
                Message = "Сброс информации о валютах",
                Time = DateTime.Now
            };

            await _historyRepo.Add(entry);
        }

        public async Task ResetStates(TimeSpan olderThan)
        {
            var now = DateTime.Now;

            var states = _stateRepo.GetAll();
            foreach (var state in states)
            {
                if (now > state.Created + olderThan)
                {
                    await _stateRepo.Delete(state);
                }
            }
        }
    }
}