using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Abstract.Workers;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;
using CurrencyTrackerServer.Infrastructure.Entities.Data;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete
{
    public class ChangeMonitor : IMonitor<IEnumerable<BaseChangeEntity>>

    {
        private readonly IWorker<IEnumerable<CurrencyChangeApiData>> _changeWorker;
        private readonly IRepositoryFactory _repoFactory;
        private readonly ISettingsProvider _settingsProvider;

        public ChangeSettings Settings => _settingsProvider.GetSettings<ChangeSettings>(Source, Destination, UserId);

        public virtual UpdateSource Source { get; set; }
        public UpdateDestination Destination => UpdateDestination.CurrencyChange;
        public string UserId { get; }

        public event EventHandler<IEnumerable<BaseChangeEntity>> Changed;

        private object _lockObject = new object();
        private bool _processing;

        public ChangeMonitor(IWorker<IEnumerable<CurrencyChangeApiData>> changeWorker,
            IRepositoryFactory repoFactory, ISettingsProvider settingsProvider, string userId)
        {
            _changeWorker = changeWorker;
            _repoFactory = repoFactory;
            _settingsProvider = settingsProvider;
            UserId = userId;
            changeWorker.Updated += ChangeWorkerOnUpdated;
        }

        private async void ChangeWorkerOnUpdated(object sender, IEnumerable<CurrencyChangeApiData> currencyChangeApiData)
        {
            if (_processing)
            {
                return; // Skip a step if processing takes longer than timer period
            }

            lock (_lockObject)
            {
                if (!_processing)
                {
                    _processing = true;
                }
                else
                {
                    return;
                }
            }

            try
            {
                await ResetStates(TimeSpan.FromHours(Settings.ResetHours));

                var changes = await CheckChanges(currencyChangeApiData);
                if (changes.Any())
                {
                    var handler = Changed;
                    handler?.Invoke(this, changes);
                }
            }
            finally
            {
                lock (_lockObject)
                {
                    _processing = false;
                }
            }
        }


        public async Task<IEnumerable<Change>> CheckChanges(IEnumerable<CurrencyChangeApiData> currencies)
        {
            if (currencies != null && !currencies.Any())
            {
                return new List<Change>(0);
            }

            var settings = Settings;
            var now = DateTime.Now;
            var changes = new List<Change>();

            var states = GetStates();

            foreach (var currency in currencies)
            {
                var isMargin = false;
                var percentage = settings.Percentage;

                if (settings.MarginCurrencies != null &&
                    settings.MarginCurrencies.Contains(currency.Currency, StringComparer.OrdinalIgnoreCase))
                {
                    percentage = settings.MarginPercentage;
                    isMargin = true;
                }

                var threshold = 0d;
                var lastChange = DateTimeOffset.MinValue;

                var state = states.FirstOrDefault(s =>
                    string.Equals(s.Currency, currency.Currency, StringComparison.OrdinalIgnoreCase) &&
                    s.UpdateSource == currency.UpdateSource);

                if (state != null)
                {
                    threshold = state.Threshold;
                    lastChange = state.LastChangeTime;
                }

                if (!isMargin && currency.PercentChanged < threshold + percentage)
                {
                    continue;
                }

                if (isMargin)
                {
                    if (threshold > 0 && currency.PercentChanged < threshold + percentage)
                    {
                        continue;
                    }
                    else if (threshold < 0 && currency.PercentChanged > threshold - percentage)
                    {
                        continue;
                    }
                    else if (threshold == 0 && Math.Abs(currency.PercentChanged) < percentage)
                    {
                        continue;
                    }
                }

                var change = new Change
                {
                    Currency = currency.Currency,
                    Percentage = currency.PercentChanged,
                    Time = now,
                    Type = UpdateType.Currency,
                    Threshold = CurrencyState.CalculateThreshold(percentage, currency.PercentChanged),
                    Source = currency.UpdateSource
                };

                if (settings.SeparateSmallerChanges && currency.PercentChanged < settings.SeparatePercentage)
                {
                    change.IsSmaller = true;
                }

                await SaveState(change);

                if (settings.MultipleChanges &&
                    now - lastChange > TimeSpan.FromMinutes(settings.MultipleChangesSpanMinutes))
                {
                    continue;
                }

                if (isMargin && settings.MultipleChanges)
                {
                    int thresholdSign = Math.Sign(threshold);
                    int changeSign = Math.Sign(currency.PercentChanged);

                    if (thresholdSign != changeSign)
                    {
                        continue;
                    }
                }

                changes.Add(change);
            }
            await SaveHistory(changes);
            return changes;
        }

        protected async Task SaveState(Change change)
        {
            if (change.Type != UpdateType.Currency) return;

            using (var repo = _repoFactory.Create<CurrencyState>())
            {
                var state = repo.GetAll().Where(s => s.UserId == UserId).FirstOrDefault(s =>
                     s.Currency == change.Currency && s.UpdateSource == change.Source);

                if (state == null)
                {
                    await repo.Add(new CurrencyState
                    {
                        UserId = UserId,
                        Currency = change.Currency,
                        LastChangeTime = change.Time.GetValueOrDefault(),
                        Threshold = change.Threshold,
                        Created = DateTime.Now,
                        UpdateSource = change.Source
                    });
                }
                else
                {
                    state.LastChangeTime = change.Time.GetValueOrDefault();
                    state.Threshold = change.Threshold;
                    await repo.Update(state);
                }
            }
        }

        protected async Task SaveHistory(IEnumerable<Change> changes)
        {
            using (var repo = _repoFactory.Create<ChangeHistoryEntry>())
            {
                foreach (var change in changes)
                {
                    var entry = new ChangeHistoryEntry
                    {
                        UserId = UserId,
                        Currency = change.Currency,
                        Message = change.Message,
                        Percentage = change.Percentage,
                        Time = change.Time.GetValueOrDefault(),
                        Type = change.Type,
                        UpdateSource = change.Source,
                        IsSmaller = change.IsSmaller
                    };

                    await repo.Add(entry, false);
                }
                await repo.SaveChanges();
            }
        }

        protected IEnumerable<CurrencyState> GetStates()
        {
            using (var repo = _repoFactory.Create<CurrencyState>())
            {
                return repo.GetAll().Where(s => s.UserId == UserId).ToList();
            }
        }

        public async Task<IEnumerable<Change>> GetHistory()
        {
            IEnumerable<ChangeHistoryEntry> entities;
            using (var repo = _repoFactory.Create<ChangeHistoryEntry>())
            {
                var history = repo.GetAll().Where(c => c.UserId == UserId && c.UpdateSource == Source);
                entities = history.ToList();

                var toDelete = history.Take(Math.Max(0, history.Count() - 50));
                foreach (var entry in toDelete)
                {
                    await repo.Delete(entry, false);
                }
                await repo.SaveChanges();
            }

            return entities.Select(entity => new Change
            {
                Currency = entity.Currency,
                Message = entity.Message,
                Percentage = entity.Percentage,
                Time = entity.Time,
                Type = entity.Type,
                Source = entity.UpdateSource,
                IsSmaller = entity.IsSmaller
            })
                .ToList();
        }

        public async Task ResetAll()
        {
            using (var repo = _repoFactory.Create<CurrencyState>())
            {
                var states = repo.GetAll().Where(s => s.UserId == UserId && s.UpdateSource == Source);
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

            using (var historyRepo = _repoFactory.Create<ChangeHistoryEntry>())
            {
                var entries = historyRepo.GetAll().Where(c => c.UserId == UserId && c.UpdateSource == Source);

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
                    var entry = new ChangeHistoryEntry
                    {
                        UserId = UserId,
                        Type = UpdateType.Info,
                        Message = "Сброс информации о валютах",
                        Time = DateTime.Now,
                        UpdateSource = Source
                    };
                    await historyRepo.Add(entry);
                }
            }
        }

        public async Task ResetStates(TimeSpan olderThan)
        {
            var now = DateTime.Now;

            using (var stateRepo = _repoFactory.Create<CurrencyState>())
            {
                var states = stateRepo.GetAll().Where(c => c.UserId == UserId && c.UpdateSource == Source);
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

        public void Dispose()
        {
            _changeWorker.Updated -= ChangeWorkerOnUpdated;
        }
    }
}