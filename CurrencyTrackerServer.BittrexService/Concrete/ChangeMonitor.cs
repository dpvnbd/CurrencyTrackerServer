using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Data;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete
{
    public class ChangeMonitor : IChangeMonitor<IEnumerable<Change>>


    {
        private IDataSource<IEnumerable<CurrencyChangeApiData>> _dataSource;
        private readonly RepositoryFactory _repoFactory;
        public ChangeSettings Settings { get; }
        public ChangeSource Source { get; }

        public ChangeMonitor(IDataSource<IEnumerable<CurrencyChangeApiData>> dataSource,
            RepositoryFactory repoFactory, IChangeSettingsProvider settingsProvider)
        {
            _dataSource = dataSource;
            this._repoFactory = repoFactory;
            Source = dataSource.Source;
            Settings = settingsProvider.GetSettings(Source);
        }


        public async Task<IEnumerable<Change>> GetChanges()
        {
            var currencies = await _dataSource.GetData();
            if (currencies != null && !currencies.Any())
            {
                return new List<Change>(0);
            }

            var now = DateTime.Now;
            var changes = new List<Change>();

            List<CurrencyStateEntity> states;

            using (var repo = _repoFactory.Create<CurrencyStateEntity>())
            {
                states = repo.GetAll().ToList();
            }

            foreach (var currency in currencies)
            {
                var percentage = Settings.Percentage;

                if (Settings.MarginCurrencies.Contains(currency.Currency, StringComparer.OrdinalIgnoreCase))
                {
                    percentage = Settings.MarginPercentage;
                }

                var threshold = 0d;
                var lastChange = DateTime.MinValue;

                var state = states.FirstOrDefault(s =>
                    s.Currency == currency.Currency && s.ChangeSource == currency.ChangeSource);

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

                await SaveState(change);

                if (Settings.MultipleChanges &&
                    now - lastChange > TimeSpan.FromMinutes(Settings.MultipleChangesSpanMinutes))
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
            if (change.Type != ChangeType.Currency) return;

            using (var repo = _repoFactory.Create<CurrencyStateEntity>())
            {
                var state = repo.GetAll().FirstOrDefault(s =>
                    s.Currency == change.Currency && s.ChangeSource == change.ChangeSource);

                if (state == null)
                {
                    await repo.Add(new CurrencyStateEntity
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
                    await repo.Update(state);
                }
            }
        }

        protected async Task SaveHistory(IEnumerable<Change> changes)
        {
            using (var repo = _repoFactory.Create<ChangeHistoryEntryEntity>())
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


                    await repo.Add(entry, false);
                }
                await repo.SaveChanges();
            }
        }

        public IEnumerable<Change> GetHistory(bool allHistory = false)
        {
            List<ChangeHistoryEntryEntity> entities;
            using (var repo = _repoFactory.Create<ChangeHistoryEntryEntity>())
            {
                var enumerable = repo.GetAll().Where(c => !allHistory && c.ChangeSource == Source);
                entities = new List<ChangeHistoryEntryEntity>(enumerable.Skip(Math.Max(0, enumerable.Count() - 50)));
            }

            return entities.Select(entity => new Change
                {
                    Currency = entity.Currency,
                    Message = entity.Message,
                    Percentage = entity.Percentage,
                    Time = entity.Time,
                    Type = entity.Type,
                    ChangeSource = entity.ChangeSource
                })
                .ToList();
        }

        public async Task ResetAll()
        {
            using (var repo = _repoFactory.Create<CurrencyStateEntity>())
            {
                var states = repo.GetAll().Where(s => s.ChangeSource == Source);
                foreach (var entity in states)
                {
                    await repo.Delete(entity, false);
                }
                await repo.SaveChanges();
            }

            await ClearHistory(TimeSpan.Zero, true);


        }

        public async Task ClearHistory(TimeSpan olderThan, bool logDeletion = false)
        {
            var now = DateTime.Now;

            using (var historyRepo = _repoFactory.Create<ChangeHistoryEntryEntity>())
            {
                var entries = historyRepo.GetAll().Where(c => c.ChangeSource == Source);

                foreach (var entry in entries)
                {
                    if (now > entry.Time + olderThan)
                    {
                        await historyRepo.Delete(entry, false);
                    }
                }

                await historyRepo.SaveChanges();

                if (logDeletion)
                {
                    var entry = new ChangeHistoryEntryEntity
                    {
                        Type = ChangeType.Info,
                        Message = "Сброс информации о валютах",
                        Time = DateTime.Now,
                        ChangeSource = Source
                    };
                    await historyRepo.Add(entry);
                }
            }
        }

        public async Task ResetStates(TimeSpan olderThan)
        {
            var now = DateTime.Now;

            using (var stateRepo = _repoFactory.Create<CurrencyStateEntity>())
            {
                var states = stateRepo.GetAll().Where(c => c.ChangeSource == Source);
                foreach (var state in states)
                {
                    if (now > state.Created + olderThan)
                    {
                        await stateRepo.Delete(state, false);
                    }
                }
                await stateRepo.SaveChanges();
            }
        }
    }
}