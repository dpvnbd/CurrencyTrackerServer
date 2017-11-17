using System.Collections.Generic;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Bittrex;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;
using Microsoft.Extensions.Options;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.Poloniex
{
    public class PoloniexTimerWorker:ChangeTimerWorker
    {
        public PoloniexTimerWorker(PoloniexApiDataSource dataSource, INotifier notifier, 
            ISettingsProvider settingsProvider, IRepositoryFactory repoFactory, IOptions<AppSettings> config)
            : base(dataSource, notifier, settingsProvider, repoFactory, config)
        {
        }

        public override UpdateSource Source => UpdateSource.Poloniex;
    }
}
