using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Abstract.Workers;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;
using CurrencyTrackerServer.Infrastructure.Entities.Data;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete
{
    public abstract class ChangeTimerWorker : AbstractTimerWorker<IEnumerable<CurrencyChangeApiData>>
    {
        private readonly IDataSource<IEnumerable<CurrencyChangeApiData>> _dataSource;
        private readonly INotifier _notifier;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IRepositoryFactory _repoFactory;

        public ChangeTimerWorker(IDataSource<IEnumerable<CurrencyChangeApiData>> dataSource, INotifier notifier,
                ISettingsProvider settingsProvider, IRepositoryFactory repoFactory)
        {
            _dataSource = dataSource;
            _notifier = notifier;
            _settingsProvider = settingsProvider;
            _repoFactory = repoFactory;
        }


        protected override async Task DoWork()
        {
            try
            {
                await ResetStates(TimeSpan.FromHours(24));
                var changes = await _dataSource.GetData();
                if (changes.Any())
                    OnUpdated(changes);
            }
            catch (Exception e)
            {
                var errorMessage = new Change
                {
                    Type = UpdateType.Error,
                    Message = e.Message,
                    Time = DateTime.Now,
                    Source = Source
                };

                await _notifier.SendToAll(new[] { errorMessage });
            }
            finally
            {
                // TODO get period settings from config
                //Period = _settingsProvider.GetSettings(Source).PeriodSeconds * 1000;
                Period = 3 * 1000;
            }
        }

        public IEnumerable<Change> GetHistory(bool allHistory = false)
        {
            List<ChangeHistoryEntry> entities;
            using (var repo = _repoFactory.Create<ChangeHistoryEntry>())
            {
                var enumerable = repo.GetAll().Where(c => !allHistory && c.UpdateSource == Source);
                entities = new List<ChangeHistoryEntry>(enumerable.Skip(Math.Max(0, enumerable.Count() - 50)));
            }

            return entities.Select(entity => new Change
            {
                Currency = entity.Currency,
                Message = entity.Message,
                Percentage = entity.Percentage,
                Time = entity.Time,
                Type = entity.Type,
                Source = entity.UpdateSource
            })
                .ToList();
        }

        public async Task ResetAll()
        {
            using (var repo = _repoFactory.Create<CurrencyState>())
            {
                var states = repo.GetAll().Where(s => s.UpdateSource == Source);
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
                var entries = historyRepo.GetAll().Where(c => c.UpdateSource == Source);

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
                var states = stateRepo.GetAll().Where(c => c.UpdateSource == Source);
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