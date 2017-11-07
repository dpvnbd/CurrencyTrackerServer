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
        private readonly IRepositoryFactory _repoFactory;
        private readonly ISettingsProvider _settingsProvider;

        public ChangeSettings Settings => _settingsProvider.GetSettings<ChangeSettings>(Source, Destination, UserId);

        public virtual UpdateSource Source { get; set; }
        public UpdateDestination Destination => UpdateDestination.CurrencyChange;
        public string UserId { get; }

        public event EventHandler<IEnumerable<BaseChangeEntity>> Changed;

        public ChangeMonitor(IWorker<IEnumerable<CurrencyChangeApiData>> changeWorker,
            IRepositoryFactory repoFactory, ISettingsProvider settingsProvider, string userId)
        {
            _repoFactory = repoFactory;
            _settingsProvider = settingsProvider;
            UserId = userId;
            changeWorker.Updated += ChangeWorkerOnUpdated;
        }

        private async void ChangeWorkerOnUpdated(object sender, IEnumerable<CurrencyChangeApiData> currencyChangeApiData)
        {
            var changes = await CheckChanges(currencyChangeApiData);
            if (changes.Any())
            {
                var handler = Changed;
                handler?.Invoke(this, changes);
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

            List<CurrencyState> states;

            using (var repo = _repoFactory.Create<CurrencyState>())
            {
                states = repo.GetAll().ToList();
            }

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
                var lastChange = DateTime.MinValue;

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
                        continue; ;
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
                var state = repo.GetAll().FirstOrDefault(s =>
                    s.Currency == change.Currency && s.UpdateSource == change.Source);

                if (state == null)
                {
                    await repo.Add(new CurrencyState
                    {
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
                        Currency = change.Currency,
                        Message = change.Message,
                        Percentage = change.Percentage,
                        Time = change.Time.GetValueOrDefault(),
                        Type = change.Type,
                        UpdateSource = change.Source
                    };


                    await repo.Add(entry, false);
                }
                await repo.SaveChanges();
            }
        }

       
    }
}