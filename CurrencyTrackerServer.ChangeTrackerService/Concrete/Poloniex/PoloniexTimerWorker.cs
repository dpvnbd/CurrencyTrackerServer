using System.Collections.Generic;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Bittrex;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.Poloniex
{
    public class PoloniexTimerWorker:ChangeTimerWorker
    {
        public PoloniexTimerWorker(PoloniexApiDataSource dataSource, INotifier notifier, 
            ISettingsProvider settingsProvider, IRepositoryFactory repoFactory)
            : base(dataSource, notifier, settingsProvider, repoFactory)
        {
        }

        public override UpdateSource Source => UpdateSource.Poloniex;
    }
}
