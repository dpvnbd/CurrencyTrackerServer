using System.Collections.Generic;
using CurrencyTrackerServer.ChangeTrackerService.Entities;
using CurrencyTrackerServer.Infrastructure.Abstract.Data;
using CurrencyTrackerServer.Infrastructure.Abstract.Workers;
using CurrencyTrackerServer.Infrastructure.Entities;
using CurrencyTrackerServer.Infrastructure.Entities.Changes;

namespace CurrencyTrackerServer.ChangeTrackerService.Concrete.Poloniex
{
    public class PoloniexChangeMonitor:ChangeMonitor
    {
        public PoloniexChangeMonitor(PoloniexTimerWorker changeWorker, IRepositoryFactory repoFactory,
            ISettingsProvider settingsProvider, string userId) : base(changeWorker, repoFactory, settingsProvider, userId)
        {
        }

        public override UpdateSource Source => UpdateSource.Poloniex;
    }
}
