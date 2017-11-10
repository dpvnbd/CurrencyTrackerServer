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
        
    }
}